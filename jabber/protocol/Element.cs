/* --------------------------------------------------------------------------
 *
 * License
 *
 * The contents of this file are subject to the Jabber Open Source License
 * Version 1.0 (the "License").  You may not copy or use this file, in either
 * source code or executable form, except in compliance with the License.  You
 * may obtain a copy of the License at http://www.jabber.com/license/ or at
 * http://www.opensource.org/.  
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied.  See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * Copyrights
 * 
 * Portions created by or assigned to Cursive Systems, Inc. are 
 * Copyright (c) 2002 Cursive Systems, Inc.  All Rights Reserved.  Contact
 * information for Cursive Systems, Inc. is available at http://www.cursive.net/.
 *
 * Portions Copyright (c) 2002 Joe Hildebrand.
 * 
 * Acknowledgements
 * 
 * Special thanks to the Jabber Open Source Contributors for their
 * suggestions and support of Jabber.
 * 
 * --------------------------------------------------------------------------*/
using System;
using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using bedrock.util;
namespace jabber.protocol
{
    /// <summary>
    /// An XmlElement with type-safe accessors.  This class is not much use by itself,
    /// but provides a number of utility functions for its descendants.
    /// </summary>
    [RCS(@"$Header$")]
    public class Element : XmlElement
    {
        /// <summary>
        /// UTF-8 encoding used throughout.
        /// </summary>
        protected static readonly Encoding ENCODING = Encoding.UTF8;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public Element(string prefix, XmlQualifiedName qname, XmlDocument doc) :
            base(prefix, qname.Name, qname.Namespace, doc)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="doc"></param>
        public Element(string localName, XmlDocument doc) :
            base(null, localName, null, doc)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="namespaceURI"></param>
        /// <param name="doc"></param>
        public Element(string localName, string namespaceURI, XmlDocument doc) :
            base(null, localName, namespaceURI, doc)
        {
        }
        /// <summary>
        /// Add a child element.  The element can be from a different document.
        /// </summary>
        /// <param name="value"></param>
        public void AddChild(XmlElement value)
        {
            if (this.OwnerDocument == value.OwnerDocument)
            {
                this.AppendChild(value);
            }
            else
            {
                this.AppendChild(this.OwnerDocument.ImportNode(value, true));
            }
        }
        /// <summary>
        /// Get the text contents of a sub-element.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string GetElem(string name)
        {
            XmlElement e = this[name];
            if (e == null)
                return null;
            if (!e.HasChildNodes)
                return null;
            return e.InnerText;
        }
        /// <summary>
        /// Set the text contents of a sub-element.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void SetElem(string name, string value)
        {
            XmlElement e = this[name];
            if (e != null)
                this.RemoveChild(e);

            e = this.OwnerDocument.CreateElement(null, name, this.NamespaceURI);
            this.AppendChild(e);

            if (value != null)
                e.InnerText = value;
        }
        
        /// <summary>
        /// Replace this first element of the same name 
        /// as the given element with the given element.
        /// </summary>
        /// <param name="elem">The new element</param>
        /// <returns>The replaced element</returns>
        protected XmlElement ReplaceChild(XmlElement elem)
        {
            XmlElement old = this[elem.Name];
            if (old != null)
            {
                this.RemoveChild(old);
            }

            AddChild(elem);
            return old;
        }

        /// <summary>
        /// Remove a child element
        /// </summary>
        /// <param name="name"></param>
        protected void RemoveElem(string name)
        {
            XmlElement e = this[name];
            if (e != null)
                this.RemoveChild(e);
        }
        /// <summary>
        /// Remove all of the matching elements from this element.
        /// </summary>
        /// <param name="name">Element local name</param>
        protected void RemoveElems(string name)
        {
            XmlNodeList nl = GetElementsByTagName(name);
            foreach (XmlElement e in nl)
            {
                this.RemoveChild(e);
            }
        }
        /// <summary>
        /// Remove all of the matching elements from this element.
        /// </summary>
        /// <param name="name">Element local name</param>
        /// <param name="namespaceURI">Element namespace URI.</param>
        protected void RemoveElems(string name, string namespaceURI)
        {
            XmlNodeList nl = GetElementsByTagName(name, namespaceURI);
            foreach (XmlElement e in nl)
            {
                this.RemoveChild(e);
            }
        }
        /// <summary>
        /// Get the value of an attribute, as a value in the given Enum type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        protected object GetEnumAttr(string name, Type enumType)
        {
            string a = this.GetAttribute(name);
            if ((a == null) || (a == ""))
                return -1;
            try
            {
                return Enum.Parse(enumType, a, true);
            }
            catch (ArgumentException)
            {
                return -1;
            }
        }
        /// <summary>
        /// Get the value of a given attribute, as an integer.  Returns -1 for
        /// most errors.   TODO: should this throw exceptions?
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected int GetIntAttr(string name)
        {
            string a = this.GetAttribute(name);
            if ((a == null) || (a == ""))
                return -1;
            try
            {
                return int.Parse(a);
            }
            catch (FormatException)
            {
                return -1;
            }
            catch (OverflowException)
            {
                return -1;
            }
        }
        /// <summary>
        /// Convert the given array of bytes into a string, having two characters 
        /// for each byte, corresponding to the hex representation of that byte.
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public static string HexString(byte[] buf)
        {
            // it seems like there ought to be a better way to do this.
            StringBuilder sb = new StringBuilder();
            foreach (byte b in buf)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();            
        }
        /// <summary>
        /// Compute the SHA1 hash of the id and secret concatenated together.
        /// </summary>
        /// <param name="id">UTF8-encoded id</param>
        /// <param name="secret">UTF8-encoded secret</param>
        /// <returns></returns>
        public static string ShaHash(string id, string secret)
        {
            Debug.Assert(id != null);
            Debug.Assert(secret != null);
            SHA1 sha = SHA1.Create();
            byte[] hash = sha.ComputeHash(ENCODING.GetBytes(id + secret));
            return HexString(hash);
        }

        /// <summary>
        /// Compute a 0K hash
        /// </summary>
        /// <param name="password">The secret to hash in</param>
        /// <param name="token">The token to permute the hash</param>
        /// <param name="sequence">Number of times to hash</param>
        /// <returns></returns>
        public static string ZeroK(string password, string token, int sequence)
        {
            Debug.Assert(password != null);
            Debug.Assert(token != null);
            SHA1 sha = SHA1.Create();
            string hash = HexString(sha.ComputeHash(ENCODING.GetBytes(password)));
            hash = HexString(sha.ComputeHash(ENCODING.GetBytes(hash + token)));
            for (int i = 0; i < sequence; i++)
            {
                hash = HexString(sha.ComputeHash(ENCODING.GetBytes(hash)));
            }
            return hash;
        }

        /// <summary>
        /// Return a DateTime version of the given Jabber date.  Example date: 20020504T20:39:42
        /// </summary>
        /// <param name="dt">The pseudo-ISO-8601 formatted date (no milliseconds)</param>
        /// <returns>A (usually UTC) DateTime</returns>
        public static DateTime JabberDate(string dt)
        {
            return new DateTime(int.Parse(dt.Substring(0, 4)),
                int.Parse(dt.Substring(4, 2)),
                int.Parse(dt.Substring(6, 2)),
                int.Parse(dt.Substring(9,2)),
                int.Parse(dt.Substring(12,2)),
                int.Parse(dt.Substring(15,2)));
        }
        /// <summary>
        /// Get a jabber-formated date for the DateTime.   Example date: 20020504T20:39:42
        /// </summary>
        /// <param name="dt">The (usually UTC) DateTime to format</param>
        /// <returns>The pseudo-ISO-8601 formatted date (no milliseconds)</returns>
        public static string JabberDate(DateTime dt)
        {
            return string.Format("{0:yyyy}{0:MM}{0:dd}T{0:HH}:{0:mm}:{0:ss}", dt);
        }

        /// <summary>
        /// The XML for the packet.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.OuterXml;
        }

        /// <summary>
        /// Return just the start tag for the element.
        /// </summary>
        /// <returns></returns>
        public string StartTag()
        {
            StringBuilder sb = new StringBuilder("<");
            sb.Append(this.Name);
            if (this.NamespaceURI != null)
            {
                sb.Append(" xmlns");
                if (this.Prefix != null)
                {
                    sb.Append(":");
                    sb.Append(this.Prefix);
                }
                sb.Append("=\"");
                sb.Append(this.NamespaceURI);
                sb.Append("\"");
            }
            foreach (XmlAttribute attr in this.Attributes)
            {
                sb.Append(" ");
                sb.Append(attr.Name);
                sb.Append("=\"");
                sb.Append(attr.Value);
                sb.Append("\"");
            }
            sb.Append(">");
            return sb.ToString();
        }

        /// <summary>
        /// Get the first child element of this element.
        /// </summary>
        /// <returns>null if none found.</returns>
        public XmlElement GetFirstChildElement()
        {
            foreach (XmlNode n in this)
            {
                if (n.NodeType == XmlNodeType.Element)
                    return (XmlElement) n;
            }
            return null;
        }

        /// <summary>
        /// System-wide one-up counter, for numbering packets.
        /// </summary>
        static int s_counter = 0;
        /// <summary>
        /// Reset the packet ID counter.  This is ONLY to be used for test cases!   No locking!
        /// </summary>
        [Conditional("DEBUG")]
        public static void ResetID()
        {
            s_counter = 0;
        }

        /// <summary>
        /// Increment the ID counter, and get the new value.
        /// </summary>
        /// <returns>The new ID.</returns>
        public static string NextID()
        {
            System.Threading.Interlocked.Increment(ref s_counter);
            return "JN_" + s_counter.ToString();
        }
    }
}
