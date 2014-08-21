using System;
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

            // Act
            var expr =
                items
                    .Pipe(spy)
                    .Pipe(new IdentityExecutable<int>());

            // execute the enumerator twice
            expr.ToList();
            expr.ToList();

            // Assert that the mapper function is hit
            // only once per item for the spy
            var allItemsHitOnce = spy.HitCount.All(kvp => kvp.Value == 1);

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
            Assert.AreEqual(0, spy.HitCount.Count);
        }

        [TestMethod]
        public void Pipe_ShouldReturnLazyExpression_WhichToListCanExecute() {
            // Setup
            var items = Enumerable.Range(0, 10);
            var spy = new SpyExecutable<int>();

            // Act
            // Build the pipe-expression but do not execute it
            items
                .Pipe(spy)
                .Pipe(new IdentityExecutable<int>())
                .ToList();

            // Assert
            // The mapper function should not be called at all
            Assert.AreEqual(10, spy.HitCount.Count);
        }
    }
}
