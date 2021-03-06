﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Pipes4NET.Tests {
    [TestClass]
    public class ExecutableSpec {
        [TestMethod]
        public void GetItem_ShouldCacheItems() {
            // Setup
            var items = Enumerable.Range(0, 10);
            var spy = new SpyExecutable<int>();
            var expr =
                items
                    .Pipe(spy)
                    .Pipe(new IdentityExecutable<int>());

            // Act
            // execute the enumerator twice
            expr.ToList();
            expr.ToList();

            // Assert that the mapper function is hit
            // only once per item for the spy
            var allItemsHitOnce = spy.MapperHitCount.All(kvp => kvp.Value == 1);

            Assert.IsTrue(allItemsHitOnce);
        }

        [TestMethod]
        public void Pipe_ShouldReturnLazyExpression() {
            // Setup
            var items = Enumerable.Range(0, 10);
            var spy = new SpyExecutable<int>();

            // Act
            // Build the pipe-expression but do not execute it
            items
                .Pipe(spy)
                .Pipe(new IdentityExecutable<int>());

            // Assert
            // The mapper function should not be called at all
            Assert.AreEqual(0, spy.MapperHitCount.Count);
        }

        [TestMethod]
        public void Pipe_ShouldReturnIEnumerable() {
            // Setup
            var items = Enumerable.Range(0, 10);
            var spy = new SpyExecutable<int>();

            // Act
            var res =
                items
                    .Pipe(spy)
                    .Pipe(new IdentityExecutable<int>());

            // Assert
            Assert.IsInstanceOfType(res, typeof(IEnumerable<int>));
        }

        [TestMethod]
        public void Pipe_ShouldReturnIExecutable() {
            // Setup
            var items = Enumerable.Range(0, 10);
            var spy = new SpyExecutable<int>();

            // Act
            var res =
                items
                    .Pipe(spy)
                    .Pipe(new IdentityExecutable<int>());

            // Assert
            Assert.IsInstanceOfType(res, typeof(IExecutable<int>));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Pipe_IntoExecutableAlreadyWithInput_ShouldThrow() {
            // Setup
            var resWithInput = Enumerable.Range(0, 10).Pipe(new IdentityExecutable<int>());

            // Act
            Enumerable.Range(10, 20).Pipe(resWithInput);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Enumerating_WithoutSource_ShouldThrow() {
            // Setup
            var resWithoutInput = new IdentityExecutable<int>();

            // Act
            resWithoutInput.ToList();
        }
    }
}
