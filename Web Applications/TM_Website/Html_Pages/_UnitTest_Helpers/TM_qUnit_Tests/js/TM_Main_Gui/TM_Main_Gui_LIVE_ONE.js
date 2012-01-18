//TM_Main_Gui_Editor_Funcionality.js
module("TeamMentor - main HTML Page");

asyncTest("Wait for load", function() 
	{
		TM.Events.onHomePageLinksLoaded.add(start);
	});
	
asyncTest("Check if main panels where loaded", function() 
	{
		equals($(".LibraryTree"			).length, 1, "found library tree");
		equals($("#pivotPanels"			).length, 1, "found pivot panels");
		equals($("#guidanceItems"		).length, 1, "found guidance items");
		equals($("#selectedGuidanceItem").length, 1, "found selected guidance item");
		start();
	});	
	
	