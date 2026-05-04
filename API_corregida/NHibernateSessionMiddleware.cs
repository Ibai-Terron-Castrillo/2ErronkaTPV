using NHibernate;

public class NHibernateSessionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ISessionFactory _sessionFactory;

    public NHibernateSessionMiddleware(RequestDelegate next, ISessionFactory sessionFactory)
    {
        _next = next;
        _sessionFactory = sessionFactory;
    }

    public async Task Invoke(HttpContext context)
    {
        var session = _sessionFactory.OpenSession();
        NHibernate.Context.CurrentSessionContext.Bind(session);

        try
        {
            await _next(context);

            if (session.IsOpen)
                await session.FlushAsync();
        }
        finally
        {
            NHibernate.Context.CurrentSessionContext.Unbind(_sessionFactory);

            if (session.IsOpen)
                session.Dispose();
        }
    }
}
