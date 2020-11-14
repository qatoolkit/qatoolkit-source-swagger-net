using QAToolKit.Source.Swagger.Exceptions;
using System;
using Xunit;

namespace QAToolKit.Source.Swagger.Test.Exceptions
{
    public class QAToolKitSwaggerExceptionTests
    {
        [Fact]
        public void CreateExceptionTest_Successful()
        {
            var exception = new QAToolKitSwaggerException("my error");

            Assert.Equal("my error", exception.Message);
        }

        [Fact]
        public void CreateExceptionWithInnerExceptionTest_Successful()
        {
            var innerException = new Exception("Inner");
            var exception = new QAToolKitSwaggerException("my error", innerException);

            Assert.Equal("my error", exception.Message);
            Assert.Equal("Inner", innerException.Message);
        }
    }
}
