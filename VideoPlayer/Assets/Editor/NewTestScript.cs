using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.Rendering;
using UnityEngine.TestTools;

namespace Tests
{
    public class NewTestScript
    {
        private bool IsVisible;
        [SetUp]
        public void CustomeInspectorSetup()
        {
       
        }

        [Test]
        public void My1stTest()
        {
            IsVisible = false;
            Assert.IsFalse(IsVisible);
        }
    }
}
