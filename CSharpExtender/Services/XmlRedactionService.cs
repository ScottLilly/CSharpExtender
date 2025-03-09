using System;
using System.Collections.Generic;
using System.Xml;

namespace CSharpExtender.Services;

public class XmlRedactionService(List<string> redactedPaths, bool ignoreCase = false)
    : BaseRedactionService(redactedPaths, ignoreCase), IRedactionService<XmlDocument>
{
    /// <summary>
    /// Redact values in the the XmlDocument.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public XmlDocument Redact(XmlDocument obj)
    {
        throw new NotImplementedException();
    }
    public XmlDocument Redact(string text)
    {
        throw new NotImplementedException();
    }
    public string RedactToString(XmlDocument obj)
    {
        throw new NotImplementedException();
    }
    public string RedactToString(string text)
    {
        throw new NotImplementedException();
    }
}
