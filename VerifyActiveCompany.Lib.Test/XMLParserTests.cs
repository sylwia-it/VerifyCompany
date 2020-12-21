using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using VerifyActiveCompany.Lib.Test.DataHelpers;

namespace VerifyActiveCompany.Lib.Test
{

    public class XMLParserTests
    {
 
        [Test]
        public void EmptyIsEmptyResposeTest()
        {
            Assert.IsTrue(BiRResponseXMLParser.IsResponseEmpty(string.Empty));
        }

        [Test]
        public void NullIsEmptyResposeTest()
        {
            Assert.Throws<ArgumentNullException>(() => BiRResponseXMLParser.IsResponseEmpty(null));
        }

        [Test]
        public void EmptyResponseIsEmptyResposeTest()
        {
            Assert.IsTrue(BiRResponseXMLParser.IsResponseEmpty("<root>\r\n</root>"));
        }

        [Test]
        public void NonEmptyErrorResponseIsEmptyResponseTest()
        {
            Assert.IsFalse(BiRResponseXMLParser.IsResponseEmpty(XMLResponseGenerator.Error4Response));
        }

        [Test]
        public void NonEmptyCorrectResponseIsEmptyResponseTest()
        {
            Assert.IsFalse(BiRResponseXMLParser.IsResponseEmpty(XMLResponseGenerator.CorrectPrawnaCompany1DaneRaportResponse));
        }

        [Test]
        public void NullGetErrorMsg()
        {
            Assert.Throws<ArgumentNullException>(() => BiRResponseXMLParser.GetErrorMsg(null));
        }

        [Test]
        public void EmptyGetErrorMsgTest()
        {
            Assert.Throws<ArgumentNullException>(() => BiRResponseXMLParser.GetErrorMsg(string.Empty));
        }

        [Test]
        public void CorrectCompanyResponseGetErrorMsgTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => BiRResponseXMLParser.GetErrorMsg(XMLResponseGenerator.CorrectPrawnaCompany1DaneRaportResponse));
        }

        [Test]
        public void Error4ResponseGetErrorMsgTest()
        {
            Assert.AreEqual(XMLResponseGenerator.Error4ErrMessage, BiRResponseXMLParser.GetErrorMsg(XMLResponseGenerator.Error4Response));
        }

        [Test]
        public void NullContainsErrorMsg()
        {
            Assert.Throws<ArgumentNullException>(() => BiRResponseXMLParser.ContainsError(null));
        }

        [Test]
        public void EmptyContainsErrorMsg()
        {
            Assert.Throws<ArgumentNullException>(() => BiRResponseXMLParser.ContainsError(string.Empty));
        }

        [Test]
        public void CorrectCompanyContainsErrorMsg()
        {
            Assert.IsFalse(BiRResponseXMLParser.ContainsError(XMLResponseGenerator.CorrectPrawnaCompany1DaneRaportResponse));
        }

        [Test]
        public void Error4ContainsErrorMsg()
        {
            Assert.IsTrue(BiRResponseXMLParser.ContainsError(XMLResponseGenerator.Error4Response));
        }


    }
}
