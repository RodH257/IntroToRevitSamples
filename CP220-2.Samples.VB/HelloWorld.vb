Imports Autodesk.Revit.DB
Imports Autodesk.Revit.Attributes
Imports Autodesk.Revit.UI

''' <summary>
''' An example Hello World command by Rod Howarth (http://www.rodhowarth.com) 
''' </summary>
<Transaction(TransactionMode.Automatic)> _
<Regeneration(RegenerationOption.Manual)> _
Public Class HelloWorld
    Implements IExternalCommand
    ''' <summary>
    ''' Displays a simple hello world dialog box.
    ''' </summary>
    ''' <param name="commandData">the revit command data</param>
    ''' <param name="message">a message to return</param>
    ''' <param name="elements">Elements to display in a failed message dialog</param>
    ''' <returns>Suceeded, Failed or Cancelled</returns>
    Public Function Execute(ByVal commandData As ExternalCommandData, _
                            ByRef message As String, ByVal elements As ElementSet) As Result _
                            Implements IExternalCommand.Execute
        'Use the TaskDialog class to get you a customized Revit 
        'version of the Windows Forms MessageBox. 
        'This fits in better with the Revit look and feel. 
        TaskDialog.Show("Hello World!", "Hello World!")

        'If there are no changes made to the model, return Cancelled. 
        'If you return 'Succeeded' Revit will think you changed something,
        'and ask the user to save when they exit. 
        Return Result.Cancelled

    End Function
End Class

