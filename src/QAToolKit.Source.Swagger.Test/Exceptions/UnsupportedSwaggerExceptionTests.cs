using System;
using QAToolKit.Source.Swagger.Exceptions;
using Xunit;

namespace QAToolKit.Source.Swagger.Test.Exceptions
{
    public class UnsupportedSwaggerExceptionTests
    {
        [Fact]
        public void CreateExceptionTest_Successful()
        {
            var exception = new UnsupportedSwaggerException("my error");

            Assert.Equal("my error", exception.Message);
        }

        [Fact]
        public void CreateExceptionWithInnerExceptionTest_Successful()
        {
            var innerException = new Exception("Inner");
            var exception = new UnsupportedSwaggerException("my error", innerException);

            Assert.Equal("my error", exception.Message);
            Assert.Equal("Inner", innerException.Message);
        }
    }
}