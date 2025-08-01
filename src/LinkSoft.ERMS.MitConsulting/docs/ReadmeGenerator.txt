Pro vygenerování nových tříd pro ERMS MIT extension spusťe následující příkazy:

xsd .\BlankScheme.xsd .\WSextIssdNotifications.xsd .\WSextEvents.xsd .\WSextInfo.xsd /classes /namespace:LinkSoft.ERMS.MITConsulting /outputDir:ExtensionTypes


a ve vytvořených souborech smažte reference, které jsou již v nadřazené složce:
tAutorizace
tFile
tFileLinksArray

ve vygenerovaných souborech pak ještě může být potřeba přejmenovat použití třídy tFileLinksArray na tFileLink[]