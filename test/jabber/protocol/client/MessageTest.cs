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
using System.Xml;
using NUnit.Framework;

using bedrock.util;
using jabber.protocol;
using jabber.protocol.client;

namespace test.jabber.protocol.client
{
    /// <summary>
    /// Summary description for MessageTest.
    /// </summary>
    [RCS(@"$Header$")]
    [TestFixture]
    public class MessageTest
    {
        XmlDocument doc = new XmlDocument();
        [SetUp]
        private void SetUp()
        {
            Element.ResetID();
        }
        public void Test_Create()
        {
            Message msg = new Message(doc);
            msg.Html = "foo";
            // TODO: deal with the namespace problem here
            // msg.Html = "f<b>o</b>o";
            Assertion.AssertEquals("<message id=\"JN_1\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><body>foo</body></html><body>foo</body></message>", msg.ToString());
        }
        public void Test_NullBody()
        {
            Message msg = new Message(doc);
            Assertion.AssertEquals(null, msg.Body);
            msg.Body = "foo";
            Assertion.AssertEquals("foo", msg.Body);
            msg.Body = null;
            Assertion.AssertEquals(null, msg.Body);
        }
        public void Test_Normal()
        {
            Message msg = new Message(doc);
            Assertion.AssertEquals(MessageType.normal, msg.Type);
            Assertion.AssertEquals("", msg.GetAttribute("type"));
            msg.Type = MessageType.chat;
            Assertion.AssertEquals(MessageType.chat, msg.Type);
            Assertion.AssertEquals("chat", msg.GetAttribute("type"));
            msg.Type = MessageType.normal;
            Assertion.AssertEquals(MessageType.normal, msg.Type);
            Assertion.AssertEquals("", msg.GetAttribute("type"));
        }
    }
}
