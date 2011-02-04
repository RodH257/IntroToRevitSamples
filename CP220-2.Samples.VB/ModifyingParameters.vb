Imports Autodesk.Revit.UI
Imports Autodesk.Revit.Attributes
Imports Autodesk.Revit.DB


''' <summary>
''' Simple parameter modification sample by Rod Howarth (http://www.rodhowarth.com) 
''' </summary>
<Transaction(TransactionMode.Automatic)> _
<Regeneration(RegenerationOption.Manual)> _
Class ModifyingParameters
    Implements IExternalCommand
    ''' <summary>
    ''' Adds 1 to the mark of every column in the document
    ''' </summary>
    ''' <param name="commandData">the revit command data</param>
    ''' <param name="message">a message to return</param>
    ''' <param name="elements">Elements to display in a failed message dialog</param>
    ''' <returns>Suceeded, Failed or Cancelled</returns>
    Public Function Execute(ByVal commandData As ExternalCommandData, _
                            ByRef message As String, ByVal elements As ElementSet) As Result _
                             Implements IExternalCommand.Execute
        'we want the 'database' document, which represents the Revit Model itself. 
        Dim dbDoc As Document = commandData.Application.ActiveUIDocument.Document

        'Use a FilteredElementCollector to get all the columns in the model.
        Dim collector As New FilteredElementCollector(dbDoc)
        'for this, we use a Category Filter which finds all elements in the Column category
        Dim columnFilter As New ElementCategoryFilter(BuiltInCategory.OST_Columns)
        'structural columns have a different category
        Dim structuralColumnFilter As New ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns)

        'to get elements that match either of these categories, we us a logical 'Or' filter. 
        Dim orFilter As New LogicalOrFilter(columnFilter, structuralColumnFilter)

        'you can then get these elements as a list.
        'we also specify WhereElementIsNotElementType() so that we don't get FamilySymbols
        Dim allColumns As IList(Of Element) = collector.WherePasses(orFilter).WhereElementIsNotElementType().ToElements()

        Dim results As String = "Updated Marks For : " & Environment.NewLine
        'loop through this list and update the mark 
        For Each element As Element In allColumns
            UpdateMark(element)
            results &= element.Id.ToString() & Environment.NewLine
        Next

        TaskDialog.Show("Updated Elements", results)

        Return Result.Succeeded
    End Function

    ''' <summary>
    ''' Adds 1 to the current mark parameter of the specified element
    ''' Assumes the mark is only numeric. 
    ''' </summary>
    ''' <param name="element"></param>
    Private Sub UpdateMark(ByVal element As Element)
        'loop through all the parameters
        For Each parameter As Parameter In element.Parameters
            'find the 'mark' parameter.
            If parameter.Definition.Name.ToLower().Equals("mark") Then
                'found the mark parameter
                ' get its current value
                Dim currentMark As String = parameter.AsString()

                'Check that the mark is a number
                Dim currentMarkNumber As Integer = 0
                If Integer.TryParse(currentMark, currentMarkNumber) Then
                    'it is, so add 1 too it. 
                    currentMarkNumber += 1

                    'set the parameter to be the new number, but remember, the mark is a text based parameter
                    'so you need to convert it to a string first, otherwise it won't change anything. 
                    parameter.Set(currentMarkNumber.ToString())
                End If
            End If
        Next
    End Sub
End Class

