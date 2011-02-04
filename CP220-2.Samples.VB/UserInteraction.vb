Imports Autodesk.Revit.Attributes
Imports Autodesk.Revit.DB
Imports Autodesk.Revit.UI
Imports Autodesk.Revit.UI.Selection

''' <summary>
''' Simple user interaction example by Rod Howarth (http://www.rodhowarth.com) 
''' </summary>
<Transaction(TransactionMode.Automatic)> _
<Regeneration(RegenerationOption.Manual)> _
Class UserInteraction
    Implements IExternalCommand
    ''' <summary>
    ''' Prompts the user based on their current selection
    ''' Allows them to delete all selected elements, or filter the selection to be only walls. 
    ''' </summary>
    ''' <param name="commandData">the revit command data</param>
    ''' <param name="message">a message to return</param>
    ''' <param name="elements">Elements to display in a failed message dialog</param>
    ''' <returns>Suceeded, Failed or Cancelled</returns>
    Public Function Execute(ByVal commandData As ExternalCommandData, _
                            ByRef message As String, ByVal elements As ElementSet) As Result _
                            Implements IExternalCommand.Execute

        Dim uiDocument As UIDocument = commandData.Application.ActiveUIDocument
        Dim dbDocument As Document = uiDocument.Document
        Dim selectedElements As SelElementSet = uiDocument.Selection.Elements

        'You should always check that the user hasn't stuffed up
        'And try to give them a helpful error message instead of having your tool crash later
        If selectedElements.Size = 0 Then
            'You can quickly show a message using TaskDialog.Show
            TaskDialog.Show("ID10T Error", "You need to select elements before running this tool")

            'returning Result.Failed now will end the add-on
            Return Result.Failed
        End If

        'TaskDialog can also be used in an extended format to show Revit style dialogs
        'This example will show two options to the user
        Dim dialog As New TaskDialog("What now?")

        'Set the main text
        dialog.MainContent = "What do you want to do with these elements you have selected?"

        'You can add up to 4 'Command Links' which are options for the user to select
        dialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Select only walls")
        dialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Delete them!")

        'Calling Show() will show the dialog to the user, and wait there
        ' until they make their choice
        'This then returns a TaskDialogresult which tells us what they selected
        Dim dialogResult As TaskDialogResult = dialog.Show()

        'Now, we need to perform actions based on what the user selected
        If dialogResult = TaskDialogResult.CommandLink1 Then
            'Asked to select only walls
            SelectOnlyWalls(selectedElements, uiDocument)
        End If

        If dialogResult = TaskDialogResult.CommandLink2 Then
            'they want to delete all of them, tell the document to delete them
            dbDocument.Delete(selectedElements)
        End If

        Return Result.Succeeded
    End Function


    ''' <summary>
    ''' Select only walls
    ''' </summary>
    ''' <param name="selectedElementSet">The users selection</param>
    ''' <param name="doc">The UIDocument currently open.</param>
    Private Sub SelectOnlyWalls(ByVal selectedElementSet As SelElementSet, _
                                ByVal doc As UIDocument)
        'Create a new selection set 
        Dim newSelection As SelElementSet = SelElementSet.Create()

        'Loop through the old selection set
        For Each element As Element In selectedElementSet
            'If the element is a Wall (these are identified by their class,
            ' as they are system families)
            If TypeOf element Is Wall Then
                'add it to the new selection
                newSelection.Add(element)
            End If
        Next
        'Overwrite the users selection with our new selection set 
        doc.Selection.Elements = newSelection
    End Sub
End Class
