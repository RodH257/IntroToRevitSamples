using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace CP220_2.Samples.CSharp
{
    /// <summary>
    /// Simple user interaction example by Rod Howarth (http://www.rodhowarth.com) 
    /// </summary>
    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    class UserInteraction : IExternalCommand
    {
        /// <summary>
        /// Prompts the user based on their current selection
        /// Allows them to delete all selected elements, or filter the selection to be only walls. 
        /// </summary>
        /// <param name="commandData">the revit command data</param>
        /// <param name="message">a message to return</param>
        /// <param name="elements">Elements to display in a failed message dialog</param>
        /// <returns>Suceeded, Failed or Cancelled</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDocument = commandData.Application.ActiveUIDocument;
            Document dbDocument = uiDocument.Document;
            SelElementSet selectedElements = uiDocument.Selection.Elements;

            //You should always check that the user hasn't stuffed up
            //And try to give them a helpful error message instead of having your tool crash later
            if (selectedElements.Size == 0)
            {
                //You can quickly show a message using TaskDialog.Show
                TaskDialog.Show("ID10T Error", "You need to select elements before running this tool");
                
                //returning Result.Failed now will end the add-on
                return Result.Failed;
            }

            //TaskDialog can also be used in an extended format to show Revit style dialogs
            //This example will show two options to the user
            TaskDialog dialog = new TaskDialog("What now?");

            //Set the main text
            dialog.MainContent = "What do you want to do with these elements you have selected?";

            //You can add up to 4 'Command Links' which are options for the user to select
            dialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Select only walls");
            dialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Delete them!");

            //Calling Show() will show the dialog to the user, and wait there until they make their choice
            //This then returns a TaskDialogresult which tells us what they selected
            TaskDialogResult result = dialog.Show();

            //Now, we need to perform actions based on what the user selected
            if (result == TaskDialogResult.CommandLink1)
            {
                //Asked to select only walls
                SelectOnlyWalls(selectedElements, uiDocument);
            }

            if (result == TaskDialogResult.CommandLink2)
            {
                //they want to delete all of them, tell the document to delete them
                dbDocument.Delete(selectedElements);
            }

            return Result.Succeeded;
        }


        /// <summary>
        /// Select only walls
        /// </summary>
        /// <param name="selectedElementSet">The users selection</param>
        /// <param name="doc">The UIDocument currently open.</param>
        private void SelectOnlyWalls(SelElementSet selectedElementSet, UIDocument doc)
        {
            //Create a new selection set 
            SelElementSet newSelection = SelElementSet.Create();

            //Loop through the old selection set
            foreach (Element element in selectedElementSet)
            {
                //If the element is a Wall (these are identified by their class, as they are system families)
                if (element is Wall)
                {
                    //add it to the new selection
                    newSelection.Add(element);
                }
            }
            //Overwrite the users selection with our new selection set 
            doc.Selection.Elements = newSelection;
        }
    }
}
