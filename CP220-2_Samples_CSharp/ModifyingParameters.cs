using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CP220_2.Samples.CSharp
{
    /// <summary>
    /// Simple parameter modification sample by Rod Howarth (http://www.rodhowarth.com) 
    /// </summary>
    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    class ModifyingParameters: IExternalCommand 
    {
        /// <summary>
        /// Adds 1 to the mark of every column in the document
        /// </summary>
        /// <param name="commandData">the revit command data</param>
        /// <param name="message">a message to return</param>
        /// <param name="elements">Elements to display in a failed message dialog</param>
        /// <returns>Suceeded, Failed or Cancelled</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //we want the 'database' document, which represents the Revit Model itself. 
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            //Use a FilteredElementCollector to get all the columns in the model.
            FilteredElementCollector collector = new FilteredElementCollector(dbDoc);
            
            //for this, we use a Category Filter which finds all elements in the Column category
            ElementCategoryFilter columnFilter = new ElementCategoryFilter(BuiltInCategory.OST_Columns);

            //structural columns have a different category
            ElementCategoryFilter structuralColumnFilter =new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            
            //to get elements that match either of these categories, we us a logical 'Or' filter. 
            LogicalOrFilter orFilter = new LogicalOrFilter(columnFilter, structuralColumnFilter);

            //you can then get these elements as a list.
            //we also specify WhereElementIsNotElementType() so that we don't get FamilySymbols
            IList<Element> allColumns = collector.WherePasses(orFilter).WhereElementIsNotElementType().ToElements();

            string results = "Updated Marks For : " + Environment.NewLine;
            //loop through this list and update the mark 
            foreach (Element element in allColumns)
            {
                UpdateMark(element); 
                results += element.Id + Environment.NewLine;
            }

            TaskDialog.Show("Updated Elements", results);
            return Result.Succeeded;
        }

        /// <summary>
        /// Adds 1 to the current mark parameter of the specified element
        /// Assumes the mark is only numeric. 
        /// </summary>
        /// <param name="element"></param>
        private void UpdateMark(Element element)
        {
            //loop through all the parameters
            foreach (Parameter parameter in element.Parameters)
            {
                //find the 'mark' parameter.
                if (parameter.Definition.Name.ToLower().Equals("mark"))
                {
                    //found the mark parameter
                    // get its current value
                    string currentMark = parameter.AsString();

                    //Check that the mark is a number
                    int currentMarkNumber = 0;
                    if (int.TryParse(currentMark, out currentMarkNumber))
                    {
                        //it is, so add 1 too it. 
                        currentMarkNumber++;

                        //set the parameter to be the new number, but remember, the mark is a text based parameter
                        //so you need to convert it to a string first, otherwise it won't change anything. 
                        parameter.Set(currentMarkNumber.ToString());
                    }
                }
            }
        }
    }
}
