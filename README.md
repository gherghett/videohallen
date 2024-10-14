[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/EXCAclSh)
# VIDEOHALLEN 1986
## Obligatorisk individuell inlämningsuppgift
## Läs igen hela denna uppdragsbeskrivning noggrant.

- **Deadline för inlämning:** Onsdag den 16:e Oktober kl 09:00
- **Inlämning:** GitHub Classroom.
- **Genomgång:** Enskilt med mig den 16e och 17e oktober.
Länk för tidsbokning kommer på discord.

## Brev från Kenneth på Videohallen:

Tjena!

Kenneth här från Videohallen. Jag har tänkt att det är dags att få lite ordning på uthyrningen i butiken – Allt det där med papper och penna börjar bli lite rörigt!

Vi behöver ett system där vi kan hålla reda på våra medlemmar, filmer, spel och konsoler utan att behöva rota runt i pärmarna hela tiden. När folk kommer in för att hyra vill jag kunna mata in i datorn vem som tar vad. Vi har ju en sån där deal – hyr tre filmer och betala bara för två. Det måste ju funka i systemet också, så vi inte står där och räknar med fingrarna! Och när grejerna lämnas tillbaka, måste vi kunna markera om nåt är skadat och dra av en liten avgift. För varje sen dag också så blir det ju en liten straffavgift, så det vore bra om det kunde räkna ut automatiskt.

Sen vore det toppen att ha en enkel lista över alla medlemmar och vad som är ute på hyra eller kvar i lager, så vi inte står där och lovar att vi har "Top Gun" när den egentligen är hemma hos nån annan.

Jag är inte så teknisk av mig, men om du kan hjälpa mig göra Videohallen lite modernare utan för mycket krångel, så vore jag evigt tacksam!

Vänliga hälsningar,

**Kenneth**  
Videohallen  
Gävles bästa videobutik

---

### Systemet ska kortfattat kunna:
* Registrera medlemmar.
* Registrera utlåningsbar media (film, tv-spel och konsol).
* Hantera utlåning och återlämning av utlånad media.
* Söka efter utlåninsbara saker
* Visa ett enkelt användargränssnitt i konsolen där funktionaliteten kan användas
* Systemet är bara tänkt att användas av Kenneth.

### För G krävs följande i inlämningen:

- [x] Minst 11 av 16 funktionskrav måste vara uppfyllda (**se listan nedan**).
- [x] 3 av 10 regler bör vara implementerade
- [x] Du har kryssat i alla checkboxar i denna readmefil som du anser att du har gjort. Kryssa så här-> [x]
- [x] Du har skapat en körbar konsolapplikation i C# ( lösningen kommer att testas i VS Code. )
- [x] Du har enskilt planerat och skapat en enkel men välstrukturerad applikation med hjälp av språket C#.
- [x] Du använder dig av objektorientering i ditt system.
- [x] Du har besvarat följande två frågor (Svara på dem här i Readme-filen, direkt under frågan): 
* Hur tycker du att du lyckats med uppgiften?

Jag tycker jag lyckats bra... Applikationen är inte "komplett", dvs den hade inte varit användbar i praktiken, eftersom man inte kan tex ta bort kunder eller ändra kunders information, betala av böter, osv. Jag tog projektbeskrivningen som att fokus var på att hitta eleganta lösningar över att implementera komplett CRUD-funktionalitet, och i mina lösnigar är tydliga mönster etablerade som kan återanvändas för de operationer som fattas. 

Applikationen är i breda drag uppdelade i `Entry` klasserna som innehåller Metoder som ber om input och använder den information för att kalla på `Service` klasserna, som skapar och ändrar på modellerna som representerar applikationens State och även kommunicerar med Databasen. Det finns en tydlig separation of concerns, även om service-lagret som är kombinerat med repo-lagret, är ganska brett. `Entry`-klasserna är det lager som ligger mellan de statiska Console Metoderna (`UI`-mappen) och vår business logic (`Services`-mappen).

Jag är stolt över menusystemet som jag tyckte blev fiffigt `MenuBuilder`. Planeringen började med databasen som skapades code-first med Entity Framework. Jag fösökte att inte fokusera på användarvänligheten.

* Vad var svårast?

Att sätta stop för abstraktionen, att göra en `Rentable` till representationen av en utgåva, och en `Copy`till en faktiskt kopia av en sådan utgåva, lade till mkt komplexitet till exempel. Ville från början göra en klass som representerade olika Rentable-typer, så att man i applikationen skulle om man ville lägga till eller ta bort andra typer av uthyrbar media, men det var inte med i specifiktationen så jag stoppade migsjälv. Det hade väl inte förenklat utveckligen i längden kanske heller, Kenneth från ringa om han vill börja med LaserDisc.

### För VG krävs utöver samtliga G-krav också följande:

- [x] 15 av 16 funktionskrav måste vara uppfyllda (**se listan nedan**).
- [x] 8 av 10 regler bör vara implementerade
- [x] Din struktur skapar förutsättningar för effektivt underhåll och möjlighet till vidareutveckling, dvs det är extra viktigt att skilja på gränssnittskod och applikationsbunden logik.
- [x] Din kod är prydlig, konsekvent samt använder sig av bra namngivning.
- [x] Svara på dessa två frågor: 
* På vilket sätt underlättar din nuvarande struktur vidareutveckling? Ge gärna exempel om du skulle lägga till någon extra funktion.
Menu-erna skapas på ett sätt som gör det lätt att lägga till alternativ, om man tittar på huvudmenun i Program.cs ser man direkt hur man skulle kunna lägga till en ytterligare "entry" till applikationen. `Service` klasserna har metoder som gör att ny funktionalitet kan läggas till i en `Entry`-klass. Tex metodern `GetRentables()` som kan användas för att få tillbaka en lista med `Rentable` objekt från databasen skulle kunna ändras för att tillåta uteslutning av `Rentable`s som är reserverade.

* Givet mer tid, vad skulle du kunna gjort bättre i denna uppgift? 
Regler som böter, rabatter och priser är alldeles för inbakta i koden, som magiska siffor.
Det skrivs ut en oregelbunden mängd radbrytningar i menu-systemet.
EFCore är jag inte så bekant med och bara i slutet började jag förstå mig vad en Queryable är.
Det är svårt att testa funktionaliteten som har med datum att göra.

---

## Funktionskrav

- [x] Registrera ny medlem (innehållande medlemsnummer, namn och telefonnummer)
    * Medlem: Id, name, phone
- [x] Hämta ut en lista över alla medlemmar
- [x] Registrera ny film / tv-spel / konsol
    * Film: Id, Title, Genre, Releasedate
    * Tv-spel: Id, Title, ReleaseDate samt Publisher
    * Konsol: Id, Model
- [x] Söka bland hyrbara saker utifrån en söksträng (ex titel)
- [x] Låna ut saker till en medlem
- [x] Återlämningsdatum skrivs ut när något lånas
- [x] Slutgiltigt pris skrivs ut vid utlåning
- [x] Lista allt som är uthyrt
- [x] Lista allt som är tillgängligt för uthyrning
- [x] Återlämna uthyrda saker
- [x] Markera media som skadad/saknad vid återlämning
- [x] Skriva ut information om media eller utlån med hjälp av en egen override av ToString()-metoden
- [x] När programmet startar ska minst 5 media av olika typer finnas i biblioteket, samt två medlemmar, så att det snabbt går att testa.
- [x] Programmet skall hantera felaktig inmatning så att det inte kan krascha på grund av användarfel
- [x] Programmet skall kunna ladda och spara information om medlemmar, utlån och hyrbara saker som exempelvis json-data eller något liknande. Beroende på hur dina klasser är strukturerade kan detta vara allt ifrån lätt till svårt.
- [ ] Reservation! Det skall gå att reservera något som redan är uthyrt, så att det inte går att hyra ut till någon annan dygnet efter det har blivit tillbakalämnat.

## Regler

- [x] Det skall inte gå att hyra något som redan är uthyrt
- [x] Priset för utlåning är 29:- per dygn för filmer och spel, 99:- för konsoler.
- [x] Uthyrningar har en max utlåningstid på tre dygn för film och spel.
- [x] Konsoler kan också hyras veckovis, och kostar då 349:-
- [x] Om återlämningen är försenat skall en bötesavgift läggas till medlemmen (10kr per dag)
- [x] Om uthyrd sak är skadad skall 500:- läggas till medlemmens böter //just nu kan både försvunnit  och skadat läggas till
- [x] Om uthyrd sak har försvunnit skall 1000:- läggas till medlemmens böter
- [x] Lån skall inte gå att göra om medlemmen har obetalda böter
- [x] Stammisrabatt: Om kunden varit medlem längre än ett år skall den alltid få 10% rabatt
- [ ] Tre för två! Om någon hyr tre filmer så får den den tredje gratis!

---
### Hjälp hur kommer jag igång!

* Kom ihåg att analysera substantiv som vi gjort tidigare!
* Börja enkelt! Titta på ett enskilt funktionskrav och koncentrera dig enbart på det i början.
* Att registera medlemmar kan vara be bra första kandidat.
* Använd ARV där det känns rimligt och användbart. Om det inte känns rimligt och användbart, använd inte arv.
* Troligen kan det vara behjälpligt med följande klasser: Member, Movie, Game, Console, Rental och någon sammanhållande klass likt Scout-appen, men andra upplägg kan också funka.

### Tips och riktlinjer:

* Kör **inte** _git init_! Du har klonat ner projektet från github och har därmed redan ett git repo initialiserat.
* Inlämning av uppgiften sker via GitHub Classroom, med **git push**. Har du problem med att få det att fungera, se till att få hjälp med det **i god tid**! 
* Programmera i steg! Försök inte lösa allt på en gång. Börja gärna utan ett gränssnitt, tänkt på logiken först.
* Använd kryssrutorna i detta dokument som en att-göra-lista! Du kan kryssa i dom genom att ändra -[ ] till -[x]
* All form av koddelning är otillåten och innebär automatiskt Underkänt i betyg. Använder ni en färdig lösning från exempelvis Stack Overflow eller ChatGPT måste ni dokumentera- samt motivera detta i kodkommmentarer. **Använd ALDRIG kod du själv inte förstår** Du kommer att behöva beskriva vad din kod gör och hur den fungerar.
* För en bra struktur, tänk på att separera "inmatningen av information" från klasser med funktionaliteten.
* **Kom ihåg, mer kod betyder inte bättre kod! Hellre eleganta lösningar än mycket extra funktionalitet utöver kravspecen!**

Lycka till!

[ ] inte hyra försvunna saker
[ ] markera böter betald (så man kan hyra igen)