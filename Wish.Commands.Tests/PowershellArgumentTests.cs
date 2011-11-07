﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Wish.Commands.Tests
{
    [TestFixture]
    public class PowershellArgumentTests
    {
        private static IEnumerable<string> GetResults(string text)
        {
            var mock = new Mock<IRunner>();
            mock.Setup(o => o.WorkingDirectory).Returns(Path.Combine(Environment.CurrentDirectory, "Test"));
            var argument = new PowershellArgument(mock.Object, text);
            return argument.Complete();
        }

        [Test]
        public void DotSlashDecorator()
        {
            var results = GetResults("di");
            Assert.True(results.Contains(@".\dir1"));
        }

        [Test]
        public void DotSlashCompleteIgnoresCase()
        {
            var result = GetResults("di");
            Assert.True(result.Contains(@".\Dir2"));
        }

        [Test]
        public void DotSlashCompleteDoesntContainNonMatches()
        {
            var result = GetResults("di");
            Assert.False(result.Contains(@".\sample"));
        }

        [Test]
        public void DotSlashCompleteCorrectCount()
        {
            var result = GetResults("di");
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void DotSlashCompleteReturnsDirectoriesAndFiles()
        {
            var result = GetResults("sa").ToList();
            Assert.True(result.Contains(@".\sample"));
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void DotSlashCompleteReturnsFiles()
        {
            var result = GetResults("sa");
            Assert.True(result.Contains(@".\safiletest.txt"));
        }

        [Test]
        public void PathSpacesCompleted()
        {
            var result = GetResults("wi");
            Assert.True(result.Contains("'.\\with space'"));
        }
    }
}