﻿using System.Collections;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wish.Commands.Runner;

namespace Wish.Commands.Tests
{
    [TestFixture]
    public class CommandTests
    {
        private Command _command;

        [SetUp]
        public void Init()
        {
            _command = new Command(null, @"cd somedir");
        }

        [Test]
        public void FunctionName()
        {
            Assert.AreEqual("cd", _command.Function.Name);
        }

        [Test]
        public void ArgumentText()
        {
            Assert.AreEqual("somedir", _command.Arguments.First().PartialPath.Text);
        }

        [Test]
        public void CompleteNoArgumentReturnsFalse()
        {
            _command = new Command(null, @"func");
            Assert.IsEmpty((ICollection) _command.Complete());
        }

        private void CreateCommandWithRunner()
        {
            const string command = @"cd somedir";
            var mock = new Mock<IRunner>();
            mock.Setup(o => o.Execute(new RunnerArgs { Script = command })).Returns("testing");
            mock.Setup(o => o.WorkingDirectory).Returns(@"T:\somewhere\somedir");
            _command = new Command(mock.Object, command);
        }

        [Test]
        public void ConstructorWithRunnerFunctionName()
        {
            CreateCommandWithRunner();
            Assert.AreEqual("cd", _command.Function.Name);
        }

        [Test]
        public void ConstructorWithRunnerArgumentText()
        {
            CreateCommandWithRunner();
            Assert.AreEqual("somedir", _command.Arguments.First().PartialPath.Text);
        }

        [Test]
        public void ConstructorWithRunner()
        {
            CreateCommandWithRunner();
            Assert.AreEqual("testing", _command.Execute().Text);
        }

        [Test]
        public void TestIsExit()
        {
            CreateCommandWithRunner();
            Assert.False(_command.IsExit);
        }

        [Test]
        public void TestIsExitWithoutRunner()
        {
            var c = new Command(null, "cd");
            Assert.False(c.IsExit);
        }

        [Test]
        public void TestIsExitTrue()
        {
            var c = new Command(null, "exit");
            Assert.True(c.IsExit);
        }

        [Test]
        public void TestIsExitTrueWithoutRunner()
        {
            var c = new Command(null, "exit");
            Assert.True(c.IsExit);
        }

        [Test]
        public void TestToString()
        {
            var c = new Command(null, "cd blah");
            Assert.AreEqual("cd blah", c.ToString());
        }

        [Test]
        public void DirectoryCommandsReturnWorkingDirectory()
        {
            CreateCommandWithRunner();
            Assert.AreEqual(@"T:\somewhere\somedir", _command.Execute().WorkingDirectory);
        }

        [Test]
        public void NonDirectoryCommandsAlsoReturnWorkingDirectory()
        {
            const string command = @"command";
            var mock = new Mock<IRunner>();
            mock.Setup(o => o.Execute(new RunnerArgs { Script = command})).Returns("testing");
            mock.Setup(o => o.WorkingDirectory).Returns(@"T:\somewhere\somedir");
            _command = new Command(mock.Object, command);
            var result = _command.Execute();
            Assert.AreEqual(@"T:\somewhere\somedir", result.WorkingDirectory);
        }

        [Test]
        [Ignore]
        public void WhatHappenWhenIRunnerIsNotSpecifiedShouldNotBeAnIssue()
        {
        }
    }
}
