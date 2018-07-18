using PdfLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_PDF
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a Catalog Dictionary
            CatalogDict catalogDict = new CatalogDict();

            //Create a Page Tree Dictionary
            PageTreeDict pageTreeDict = new PageTreeDict();

            //Create a Font Dictionary
            FontDict TimesRoman = new FontDict();
            FontDict TimesItalic = new FontDict();

            //Create the info Dictionary
            InfoDict infoDict = new InfoDict();

            //Create the font called Times Roman
            TimesRoman.CreateFontDict("T1", "Times-Roman");

            //Create the font called Times Italic
            TimesItalic.CreateFontDict("T2", "Times-Italic");

            //Set the info Dictionary. 
            infoDict.SetInfo("title", "author", "company");

            //Create a utility object
            Utility pdfUtility = new Utility();

            //Open a file specifying the file name as the output pdffile 
            FileStream file = new FileStream(@"D:\text.pdf", FileMode.Create);
            file.Write(pdfUtility.GetHeader("1.5", out int size), 0, size);
            file.Close();
            //Now we finished doing the first step

            /*************************************************************/
            //Create a Page Dictionary , this represents a visible page
            PageDict page = new PageDict();
            ContentDict content = new ContentDict();

            //The page size object will hold all the page size information
            PageSize pSize = new PageSize(612, 792);
            pSize.SetMargins(10, 10, 10, 10);
            page.CreatePage(pageTreeDict.objectNum, pSize);
            pageTreeDict.AddPage(page.objectNum);
            page.AddResource(TimesRoman, content.objectNum);

            //Create a Text And Table Object that present the elements in the page
            TextAndTables textAndtable = new TextAndTables(pSize);

            //Add text to the page
            textAndtable.AddText(20, 10, "Testing", 10, "T1", Align.CenterAlign);

            //Create the array for alignment value.
            //This is specified for text in each column 
            //of the table, here we have two columns
            Align[] align = new Align[2];
            align[0] = Align.LeftAlign;
            align[1] = Align.LeftAlign;

            //Specify the color for the cell and the line
            ColorSpec cellColor = new ColorSpec(100, 100, 100);
            ColorSpec lineColor = new ColorSpec(98, 200, 200);

            //Fill in the parameters for the table
            TableParams table = new TableParams(2, 200, 200);
            table.yPos = 700;
            table.xPos = 100;
            table.rowHeight = 20;

            //Set the parameters of this table
            textAndtable.SetParams(table, cellColor, Align.CenterAlign, 3);
            textAndtable.AddRow(false, 10, "T1", align, "First Column", "Second Column");
            textAndtable.AddRow(false, 10, "T1", align, "Second Row", "Second Row");

            //Repeat till we require the number of rows.
            //After drawing table and text add them to the page 
            content.SetStream(textAndtable.EndTable(lineColor));
            content.SetStream(textAndtable.EndText());
            size = 0;
            file = new FileStream(@"D:\text.pdf", FileMode.Append);
            file.Write(page.GetPageDict(file.Length, out size), 0, size);
            file.Write(content.GetContentDict(file.Length, out size), 0, size);
            file.Close();
            /**********************************3***************************/
            //Write everything file size=0;
            file = new FileStream(@"D:\text.pdf", FileMode.Append);
            file.Write(catalogDict.GetCatalogDict(pageTreeDict.objectNum,
                                              file.Length, out size), 0, size);
            file.Write(pageTreeDict.GetPageTree(file.Length, out size), 0, size);
            file.Write(TimesRoman.GetFontDict(file.Length, out size), 0, size);
            file.Write(TimesItalic.GetFontDict(file.Length, out size), 0, size);
            file.Write(infoDict.GetInfoDict(file.Length, out size), 0, size);
            file.Write(pdfUtility.CreateXrefTable(file.Length, out size), 0, size);
            file.Write(pdfUtility.GetTrailer(catalogDict.objectNum,
                                       infoDict.objectNum, out size), 0, size);
            file.Close();

            Console.ReadKey();
        }
    }
}
