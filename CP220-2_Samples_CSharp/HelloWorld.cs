using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace CP220_2.Samples.CSharp
{
    /// <summary>
    /// An example Hello World command by Rod Howarth (http://www.rodhowarth.com) 
    /// </summary>
    [Transaction(TransactionMode.Automatic)] // Automatically creates a transaction for you
    [Regeneration(RegenerationOption.Manual)] // Won't regenerate the model unless you tell it to
    public class HelloWorld : IExternalCommand
    {
        /// <summary>
        /// Displays a simple hello world dialog box.
        /// </summary>
        /// <param name="commandData">the revit command data</param>
        /// <param name="message">a message to return</param>
        /// <param name="elements">Elements to display in a failed message dialog</param>
        /// <returns>Suceeded, Failed or Cancelled</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Use the TaskDialog class to get you a customized Revit version of the Windows Forms MessageBox. 
            //This fits in better with the Revit look and feel. 
            TaskDialog.Show("Hello World!", "Hello World!");
         
            //If there are no changes made to the model, return Cancelled. 
            //If you return 'Succeeded' Revit will think you changed something, and ask the user to save when they exit. )
            return Result.Cancelled;
        }
    }
}
