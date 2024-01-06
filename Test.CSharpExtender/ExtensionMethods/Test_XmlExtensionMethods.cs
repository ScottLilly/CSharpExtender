using System.Xml;
using CSharpExtender.ExtensionMethods;

namespace Test.CSharpExtender.ExtensionMethods
{
    public class Test_XmlExtensionMethods
    {
        [Fact]
        public void AttributeAsInt_ValidAttribute_ReturnsParsedInt()
        {
            var doc = new XmlDocument();
            doc.LoadXml("<root attr='123'></root>");
            var node = doc.DocumentElement;

            var result = node.AttributeAsInt("attr");

            Assert.Equal(123, result);
        }

        [Fact]
        public void AttributeAsInt_InvalidAttribute_ReturnsDefaultInt()
        {
            var doc = new XmlDocument();
            doc.LoadXml("<root attr='abc'></root>");
            var node = doc.DocumentElement;

            var result = node.AttributeAsInt("attr");

            Assert.Equal(default, result);
        }

        [Fact]
        public void AttributeAsString_ValidAttribute_ReturnsString()
        {
            var doc = new XmlDocument();
            doc.LoadXml("<root attr='abc'></root>");
            var node = doc.DocumentElement;

            var result = node.AttributeAsString("attr");

            Assert.Equal("abc", result);
        }

        [Fact]
        public void AttributeAsString_InvalidAttribute_ReturnsNull()
        {
            var doc = new XmlDocument();
            doc.LoadXml("<root></root>");
            var node = doc.DocumentElement;

            var result = node.AttributeAsString("attr");

            Assert.Null(result);
        }

        [Fact]
        public void AttributeAsBool_ValidAttribute_ReturnsParsedBool()
        {
            var doc = new XmlDocument();
            doc.LoadXml("<root attr='true'></root>");
            var node = doc.DocumentElement;

            var result = node.AttributeAsBool("attr");

            Assert.True(result);
        }

        [Fact]
        public void AttributeAsBool_InvalidAttribute_ReturnsDefaultBool()
        {
            var doc = new XmlDocument();
            doc.LoadXml("<root attr='abc'></root>");
            var node = doc.DocumentElement;

            var result = node.AttributeAsBool("attr");

            Assert.Equal(default, result);
        }

        [Fact]
        public void AttributeAsDateTime_ValidAttribute_ReturnsParsedDateTime()
        {
            var doc = new XmlDocument();
            doc.LoadXml("<root attr='2022-01-01'></root>");
            var node = doc.DocumentElement;

            var result = node.AttributeAsDateTime("attr");

            Assert.Equal(new DateTime(2022, 1, 1), result);
        }

        [Fact]
        public void AttributeAsDateTime_InvalidAttribute_ReturnsDefaultDateTime()
        {
            var doc = new XmlDocument();
            doc.LoadXml("<root attr='abc'></root>");
            var node = doc.DocumentElement;

            var result = node.AttributeAsDateTime("attr");

            Assert.Equal(default, result);
        }

        [Fact]
        public void ElementAsString_ValidElement_ReturnsInnerText()
        {
            var doc = new XmlDocument();
            doc.LoadXml("<root><child>abc</child></root>");
            var node = doc.DocumentElement;

            var result = node.ElementAsString("child");

            Assert.Equal("abc", result);
        }

        [Fact]
        public void ElementAsString_InvalidElement_ReturnsNull()
        {
            var doc = new XmlDocument();
            doc.LoadXml("<root></root>");
            var node = doc.DocumentElement;

            var result = node.ElementAsString("child");

            Assert.Null(result);
        }

        [Fact]
        public void ElementAsInt_ValidElement_ReturnsParsedInt()
        {
            var doc = new XmlDocument();
            doc.LoadXml("<root><child>123</child></root>");
            var node = doc.DocumentElement;

            var result = node.ElementAsInt("child");

            Assert.Equal(123, result);
        }

        [Fact]
        public void ElementAsInt_InvalidElement_ReturnsDefaultInt()
        {
            var doc = new XmlDocument();
            doc.LoadXml("<root><child>abc</child></root>");
            var node = doc.DocumentElement;

            var result = node.ElementAsInt("child");

            Assert.Equal(default, result);
        }
    }
}
