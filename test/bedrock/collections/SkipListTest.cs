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
using NUnit.Framework;
using bedrock.collections;
using bedrock.util;
using System.Collections;

namespace test.bedrock.collections
{
    /// <summary>
    /// Summary description for SkipListTest.
    /// </summary>
    [RCS(@"$Header$")]
    public class SkipListTest : TestCase
    {
        private System.Text.Encoding ENC = System.Text.Encoding.Default;
        public SkipListTest(string name) : base(name) {}

        public static ITest Suite
        {
            get { return new TestSuite(typeof(SkipListTest)); }
        }

        public void Test_Add()
        {
            SkipList sl = new SkipList();
            AssertEquals(0, sl.Count);
            sl.Add("1", "bar");
            AssertEquals(1, sl.Count);
            sl.Add("1", "baz");
            AssertEquals(1, sl.Count);
            sl.Add("2", "baz");
            AssertEquals(2, sl.Count);
        }
        public void Test_Get()
        {
            SkipList sl = new SkipList();
            sl.Add("1", "bar");
            AssertEquals("bar", sl["1"]);
        }
        public void Test_Lots_InOrder()
        {
            SkipList sl = new SkipList();
            for (int i=0; i<4096; i++)
            {
                sl[i] = i.ToString();
            }
            AssertEquals(4096, sl.Count);
            for (int i=0; i<4096; i++)
            {
                AssertEquals(i.ToString(), sl[i]);
            }
        }
        public void Test_Lots_Random()
        {
            SkipList sl = new SkipList();
            Random r = new Random();
            int[] nums = new int[4096];

            for (int i=0; i<4096; i++)
            {
                nums[i] = r.Next(10000);
                while (sl.Contains(nums[i]))
                {
                    nums[i] = r.Next(10000);
                }
                sl[nums[i]] = i.ToString();
            }
            AssertEquals(4096, sl.Count);
            for (int i=0; i<4096; i++)
            {
                AssertEquals(i.ToString(), sl[nums[i]]);
            }
        }
        public void Test_Iteration()
        {
            SkipList sl = new SkipList();
            for (int i=0; i<4096; i++)
            {
                sl[i] = i.ToString();
            }
            AssertEquals(4096, sl.Count);
            int count = 0;
            foreach (DictionaryEntry de in sl)
            {
                AssertEquals(count, de.Key);
                AssertEquals(count.ToString(), de.Value);
                count++;
            }
            AssertEquals(4096, count);
        }

        public void Test_DictIteration()
        {
            SkipList sl = new SkipList();
            for (int i=0; i<4096; i++)
            {
                sl[i] = i.ToString();
            }
            AssertEquals(4096, sl.Count);
            int count = 0;
            IDictionaryEnumerator e = sl.GetEnumerator();
            while (e.MoveNext())
            {
                AssertEquals(count, e.Key);
                AssertEquals(count.ToString(), e.Value);
                count++;
            }
            AssertEquals(4096, count);
        }
    
        public void Test_Remove()
        {
            SkipList sl = new SkipList();
            sl[0] = 0;
            AssertEquals(1, sl.Count);
            sl.Remove(0);
            AssertEquals(0, sl.Count);
        }

        public void Test_Clear()
        {
            SkipList sl = new SkipList();
            for (int i=0; i<4096; i++)
            {
                sl[i] = i.ToString();
            }
            AssertEquals(4096, sl.Count);
            sl.Clear();
            AssertEquals(0, sl.Count);
        }
    }
}
