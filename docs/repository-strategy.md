# Repository-Strategie

## Ziel

Das Repository `Robin-Goerlach/SASD-Desktop-Secret-Manager` dient als aktives Entwicklungsrepository für den SASD Desktop Secret Manager. Es darf Entwicklungsstände, Lernfortschritte, Prototypen und Zwischenentscheidungen enthalten. Für später öffentlich stärker sichtbare SASD-GmbH-Releases kann ein bereinigtes Release-Repository oder eine Spiegelung unter der SASD-GmbH-Organisation sinnvoll sein.

## Aktuelle Einordnung

Der aktuelle Stand ist technisch interessant und bereits lauffähig, aber noch nicht als produktives Sicherheitsprodukt freizugeben. Die Dokumentation beschreibt daher bewusst:

- was bereits umgesetzt ist,
- was für V1 noch fertigzustellen ist,
- was in V1.x/V2 geplant ist,
- was bewusst nicht Ziel ist.

## Arbeitsmodell

Für die weitere Entwicklung wird empfohlen:

1. Änderungen in kleinen, nachvollziehbaren Commits vornehmen.
2. Dokumentationsänderungen und Codeänderungen möglichst getrennt committen.
3. Bei sicherheitsrelevanten Änderungen immer Security-/Threat-Model und Testkatalog prüfen.
4. Für größere Entscheidungen ADRs ergänzen.
5. Vor einem Release eine saubere Release-Branch- oder Tag-Strategie verwenden.

## Release-Strategie

Ein späterer SASD-GmbH-Release sollte nur aus einem stabilen, getesteten Stand erzeugt werden. Vorher müssen mindestens erfüllt sein:

- `dotnet restore`, `dotnet build`, `dotnet test` grün,
- V1-Abnahmekatalog ohne kritische offene Punkte,
- Security-Gates erfüllt,
- README, Lastenheft, Pflichtenheft, Architektur und Roadmap konsistent,
- keine Test-Secrets, privaten Daten oder ungewollten Debug-Artefakte im Repository.

## Öffentliche Darstellung

Das öffentliche Entwicklungsrepository sollte ehrlich sein: Es darf professionell aussehen, aber es muss klar sagen, dass es sich noch nicht um ein produktiv freigegebenes Sicherheitsprodukt handelt. Gerade bei einem Secret Manager ist Transparenz wichtiger als übertriebenes Marketing.
