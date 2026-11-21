using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace SqlProfiler {
  internal static class EventExporter {
    private const string SpreadsheetNamespace = "urn:schemas-microsoft-com:office:spreadsheet";
    private const string HeaderStyleId = "s62";

    public static string BuildEventsXml(IEnumerable<ProfilerEvent> events) {
      var doc = new XmlDocument();
      XmlNode root = doc.CreateElement("events");
      foreach (var evt in events)
        root.AppendChild(CreateEventRow(doc, evt));
      doc.AppendChild(root);
      doc.PreserveWhitespace = true;
      using var writer = new StringWriter();
      using (var textWriter = new XmlTextWriter(writer) {Formatting = Formatting.Indented}) {
        doc.Save(textWriter);
      }

      return writer.ToString();
    }

    private static XmlNode CreateEventRow(XmlDocument doc, ProfilerEvent evt) {
      XmlNode row = doc.CreateElement("event");
      AddAttribute(row, "EventClass", evt.EventClass.ToString(CultureInfo.InvariantCulture));
      AddAttribute(row, "CPU", evt.CPU.ToString(CultureInfo.InvariantCulture));
      AddAttribute(row, "Reads", evt.Reads.ToString(CultureInfo.InvariantCulture));
      AddAttribute(row, "Writes", evt.Writes.ToString(CultureInfo.InvariantCulture));
      AddAttribute(row, "Duration", evt.Duration.ToString(CultureInfo.InvariantCulture));
      AddAttribute(row, "SPID", evt.SPID.ToString(CultureInfo.InvariantCulture));
      AddAttribute(row, "LoginName", evt.LoginName);
      AddAttribute(row, "DatabaseName", evt.DatabaseName);
      AddAttribute(row, "ObjectName", evt.ObjectName);
      AddAttribute(row, "ApplicationName", evt.ApplicationName);
      AddAttribute(row, "HostName", evt.HostName);
      row.InnerText = evt.TextData;
      return row;
    }

    /// <summary>Builds an "XML Spreadsheet 2003" document (opens in Excel) from the list items.</summary>
    public static XmlDocument BuildExcelDocument(ListView listView, IList<PerfColumn> columns,
      IEnumerable<ListViewItem> items, Func<ProfilerEvent, string> getCaption,
      Func<ProfilerEvent, int, string, string> getFormattedValue) {
      var doc = new XmlDocument();
      doc.AppendChild(doc.CreateProcessingInstruction("mso-application", "progid='Excel.Sheet'"));
      XmlNode root = CreateElement(doc, "Workbook");
      AddAttribute(root, "xmlns:ss", SpreadsheetNamespace);
      doc.AppendChild(root);

      var style = CreateElement(doc, "Style");
      AddSpreadsheetAttribute(style, "ID", HeaderStyleId);
      var font = CreateElement(doc, "Font");
      AddSpreadsheetAttribute(font, "Bold", "1");
      style.AppendChild(font);
      var styles = CreateElement(doc, "Styles");
      styles.AppendChild(style);
      root.AppendChild(styles);

      var worksheet = CreateElement(doc, "Worksheet");
      AddSpreadsheetAttribute(worksheet, "Name", "Sql Trace");
      root.AppendChild(worksheet);
      var table = CreateElement(doc, "Table");
      AddSpreadsheetAttribute(table, "ExpandedColumnCount", columns.Count.ToString(CultureInfo.InvariantCulture));
      worksheet.AppendChild(table);

      foreach (ColumnHeader column in listView.Columns) {
        var columnNode = CreateElement(doc, "Column");
        AddSpreadsheetAttribute(columnNode, "AutoFitWidth", "0");
        AddSpreadsheetAttribute(columnNode, "Width", column.Width.ToString(CultureInfo.InvariantCulture));
        table.AppendChild(columnNode);
      }

      var headerRow = CreateElement(doc, "Row");
      table.AppendChild(headerRow);
      foreach (ColumnHeader column in listView.Columns)
        AddCell(headerRow, "String", column.Text, HeaderStyleId);

      long rowNumber = 1;
      foreach (var item in items) {
        var evt = (ProfilerEvent) item.Tag;
        var row = CreateElement(doc, "Row");
        table.AppendChild(row);
        foreach (var pc in columns) {
          if (pc.Column == -1) {
            AddCell(row, "Number", rowNumber.ToString(CultureInfo.InvariantCulture));
          }
          else if (pc.Column == ProfilerEventColumns.EventClass) {
            AddCell(row, "String", getCaption(evt));
          }
          else {
            var type = ProfilerEventColumns.ProfilerColumnDataTypes[pc.Column];
            var isNumber = type == ProfilerColumnDataType.Int || type == ProfilerColumnDataType.Long;
            var format = type == ProfilerColumnDataType.DateTime ? pc.Format : "";
            AddCell(row, isNumber ? "Number" : "String", getFormattedValue(evt, pc.Column, format) ?? "");
          }
        }

        rowNumber++;
      }

      return doc;
    }

    /// <summary>UTF-8 bytes without BOM, so that the declared encoding matches the real one.</summary>
    public static byte[] GetXmlBytes(XmlDocument doc) {
      using var ms = new MemoryStream();
      using (var textWriter = new XmlTextWriter(ms, new UTF8Encoding(false)) {Formatting = Formatting.Indented, Namespaces = true}) {
        doc.Save(textWriter);
      }

      return ms.ToArray();
    }

    private static XmlElement CreateElement(XmlDocument doc, string name) {
      return doc.CreateElement("ss", name, SpreadsheetNamespace);
    }

    private static void AddCell(XmlNode row, string type, string text, string styleId = null) {
      var doc = row.OwnerDocument;
      var cell = CreateElement(doc, "Cell");
      if (styleId is not null) AddSpreadsheetAttribute(cell, "StyleID", styleId);
      var data = CreateElement(doc, "Data");
      AddSpreadsheetAttribute(data, "Type", type);
      data.InnerText = text;
      cell.AppendChild(data);
      row.AppendChild(cell);
    }

    private static void AddAttribute(XmlNode node, string name, string value) {
      var attribute = node.OwnerDocument.CreateAttribute(name);
      attribute.Value = value;
      node.Attributes.Append(attribute);
    }

    private static void AddSpreadsheetAttribute(XmlNode node, string name, string value) {
      var attribute = node.OwnerDocument.CreateAttribute("ss", name, SpreadsheetNamespace);
      attribute.Value = value;
      node.Attributes.Append(attribute);
    }
  }
}
