using JatetxeaApi.DTOak;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace JatetxeaApi.Repositorioak
{
    public class DeskuntuakRepository
    {
        private readonly string _connectionString;

        public DeskuntuakRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OdooPostgres")
                ?? "Host=localhost;Port=5446;Database=odoo_db;Username=odoo_db;Password=odoo_db";
        }

        public async Task<DeskuntuEmaitzaDto> ValidateAsync(string? code)
        {
            var normalized = NormalizeCode(code);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return Invalid("Kodea beharrezkoa da");
            }

            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = """
                    select id, percentage, active, usage_limit, usage_count, valid_from, valid_until
                    from jatetxeko_deskontuak
                    where upper(trim(name)) = @code
                    limit 1
                    """;
                command.Parameters.AddWithValue("code", normalized);

                await using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return Invalid("Kodea ez da aurkitu");
                }

                var id = reader.GetInt32(0);
                var percentage = Convert.ToDouble(reader.GetValue(1));
                var active = !reader.IsDBNull(2) && reader.GetBoolean(2);
                var usageLimit = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                var usageCount = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                var validFrom = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5);
                var validUntil = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);

                if (!active)
                {
                    return Invalid("Kodea ez dago aktibo");
                }

                var today = DateTime.Today;
                if (validFrom.HasValue && validFrom.Value.Date > today)
                {
                    return Invalid("Kodea oraindik ez da baliozkoa");
                }

                if (validUntil.HasValue && validUntil.Value.Date < today)
                {
                    return Invalid("Kodea iraungi da");
                }

                if (usageLimit > 0 && usageCount >= usageLimit)
                {
                    return Invalid("Kodea jada erabili da");
                }

                return new DeskuntuEmaitzaDto
                {
                    Valid = true,
                    Message = "OK",
                    Percentage = percentage,
                    CodeId = id
                };
            }
            catch (Exception ex)
            {
                return Invalid($"Errorea Odoo datu-basearekin: {ex.Message}");
            }
        }

        public async Task<DeskuntuEmaitzaDto> ApplyAsync(string? code)
        {
            var validation = await ValidateAsync(code);
            if (!validation.Valid || validation.CodeId == null)
            {
                return validation;
            }

            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = """
                    update jatetxeko_deskontuak
                    set usage_count = coalesce(usage_count, 0) + 1,
                        write_date = now()
                    where id = @id
                    """;
                command.Parameters.AddWithValue("id", validation.CodeId.Value);
                await command.ExecuteNonQueryAsync();

                return validation;
            }
            catch (Exception ex)
            {
                return Invalid($"Errorea deskontua aplikatzean: {ex.Message}");
            }
        }

        private static string NormalizeCode(string? code)
        {
            return (code ?? string.Empty).Trim().ToUpperInvariant();
        }

        private static DeskuntuEmaitzaDto Invalid(string message)
        {
            return new DeskuntuEmaitzaDto
            {
                Valid = false,
                Message = message
            };
        }
    }
}
