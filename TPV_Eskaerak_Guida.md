# TPV — Eskaerak: nola sortu eta aldatu

Gida laburra: dokumentu honek azaltzen du nola sortu eta nola aldatu bezeroaren eskaerak TPV aplikazioaren GUI bidez (erreserbetatik hasi, platerak aukeratu, eta "Eskaera Amaitu" bidali).

**Aurretik**
- Jarraitu aplikazioa martxan eta saioa hasi langile moduan.
- Menu nagusian irekiko da `ZerbitzariMenua` eta hortik `ZerbitzuaHasi` erabiliko dugu.

## 1. Eskaera sortu (pauso praktikoa)

1. Ireki `ZerbitzariMenua` eta sakatu "Zerbitzua hasi" (`ZerbitzuaHasi`).
2. `ZerbitzuaHasi` leihoak gaur egungo erreserbak erakusten ditu. Erreserbak iragazi ditzakezu orduaren arabera `hourFilter`-en bidez ("Guztiak" edo ordutegi arruntak).
3. Aukeratu bezeroaren fitxa (izena + ordutegia) — botoia sakatzean irekitzen da `ZerbitzuaKudeatu` leihoa hautatutako `ErreserbaId`, `LangileId` eta `MahaiaId` erabilita.
4. `ZerbitzuaKudeatu` leihoan katalogoaren arabera platerak ikusiko dituzu (kategoriak, irudiak, prezioak). Plater bakoitzean `+` eta `-` botoiak daude kantitatea aldatzeko.
   - `+` sakatzean, aplikazioak lehenik stock egoera egiaztatzen du (`GehienezkoStocka`) eta gehitu daitekeen gehienezko kopurua ezartzen du (stock + hasierako kantitatea).
   - `-` sakatzean, ezin da jaitsi `minimoak` kopuru azpitik (aurrez zerbitzatutako kantitatea mantentzeko).
   - Panelek kolorez adierazten dute stock egoera: gorria=0, horia=<=5, zuria=normala.
5. Platerak aukeratu ondoren, sakatu `Eskaera Amaitu`. Aplikazioak hurrengoak egingo ditu:
   - Ziurtatu zerbitzu-objektua existitzen dela (API bidez). Ez badago, sortzen saiatuko da automatikoki.
   - Era berean, bidaliko du `Platerak` zerrenda zerbitzuaren endpoint-era (`/api/Zerbitzuak/egin`).
6. Bezeroari arrakastaz bidali bada, mezua agertuko da: "Eskaera ondo egin da!".

## 2. Eskaera aldatzea (eguneratzea)

- Alde batetik, `ZerbitzuaKudeatu` irekitzean aplikazioak kargatzen ditu aurrez egindako platerak (GET `/api/Zerbitzuak/erreserba/{erreserbaId}/platerak`).
  - Aurreko platerak `hasierakoak` eta `aukerak` map-ean gordetzen dira; hauetatik abiatuta egin ahal izango dituzu aldaketak.
- Kantitateak aldatzen dituzunean, berriro sakatu `Eskaera Amaitu`. Aplikazioak bidaliko du eguneratutako `Platerak` zerrenda eta backend-ekin sinkronizatuko du eskaera.
- Kontuan izan: ezin da kopurua murriztu dagoeneko zerbitzatutako kopurutik (minimoak) behera.

## 3. Nola komunikatzen da atzealdearekin (azalpena teknikoki)

- Nagusiak erabilitako endpoint-ak (kodean agertzen direnak):
  - `GET /api/Erreserbak` — erreserbak kargatzeko (egunekoak filter bidez erakusten dira).
  - `GET /api/Platerak`, `GET /api/Kategoria`, `GET /api/Inbentarioa`, `GET /api/PlaterenOsagaiak` — platerak, kategoriak eta stock informazioa.
  - `GET /api/Zerbitzuak/erreserba/{erreserbaId}/platerak` — aurrez egindako platerak kargatzeko.
  - `POST /api/Zerbitzuak/egin` — eskaera sortu/eguneratzeko erabiltzen den endpoint-a.

- `Eskaera` payload-aren lagin tipikoa (JSON):

```json
{
  "LangileId": 3,
  "MahaiaId": 12,
  "ErreserbaId": 42,
  "Platerak": [
    { "PlateraId": 5, "Kantitatea": 2 },
    { "PlateraId": 7, "Kantitatea": 1 }
  ]
}
```

- Kontuan izatekoak: kodeak batzuetan saiakera anitz egiten ditu `Egoera` eremuarekin (adibidez: `null`, `Eskatuta`, `Egiten`, `Itxaropean`) errore zehatzak itzultzen baditu. Hau bezero-taldeko tolerantzia mekanismoa da.

## 4. Arazoak konpontzeko

- `+` botoia desgaitua badago eta panela gorria erakusten badu, plater hori prestatzeko osagaiak bukatu dira (stock 0).
- "Errorea eskaera amaitzean" mezua jasotzen baduzu, leihoa irekita mantendu eta softwareak erakusten duen errore-edukia aztertu. Sarritan API-ren erantzunak `egoera` baliogabea dela edo `Data truncated` bezalako mezua ematen du — hau aniztasunez saiatzeko tratatzen da.
- Aurreko plater kopuruak beraiek ez badira zuzenak, egiaztatu backend-eko `/api/Zerbitzuak/erreserba/{id}/platerak` funtzioa edo Inbentarioa eguneratuta dagoen.

## 5. Gomendioak eta ohar teknikoak

- Behar izanez gero, backend-eko APIak testatu eta egiaztatu Postman edo antzeko tresnarekin `POST /api/Zerbitzuak/egin` payload batekin.
- Gehienezko plater kopurua (gehieneko stock) kalkulatzen da `GehienezkoStocka` funtzioan, non plater bakoitzeko osagai-kopuruak kontuan hartzen diren (Inbentarioa / PlaterenOsagaiak).
