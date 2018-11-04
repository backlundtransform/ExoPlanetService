# ExoPlanetService
The data is provided by the Planetary Habitability Laboratory (PHL) University of Puerto Rico at Arecibo and can be found here: phl.upr.edu 

The Api is using OData protocol that allows for logical querying capabilities. E.g if you want to find habitable planets "$filter= Habitable eq true". Find planet with name 14 And b "$filter=Name eq '14 And b’". Read more here: msdn.microsoft.com. Skip and top E.g takes 30 and skips 10 "$top=30 $skip=10". Order by ascending discovery year $orderby=Disc_Year asc.
