using System;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Tests
{
    public class Set_Tests
    {
        private Set<int> a;
        private Set<int> b;

        [SetUp]
        public void Setup()
        {
            a = new Set<int>(new[] {1, 2, 3, 4});
            b = a.GetEmptySubset();
        }

        [Test]
        public void Counts()
        {
            Assert.AreEqual(0, a.Count);

            a.Include(1, 2);
            Assert.AreEqual(2, a.Count);

            var rp = a.Include(2);
            Assert.AreEqual(1, rp);
            Assert.AreEqual(2, a.Count);
        }

        [Test]
        public void Clears()
        {
            a.Include(1, 2);

            a.Clear();

            Assert.AreEqual(0, a.Count);
            Assert.That(a.AsEnumerable(), Is.Empty);
        }

        [Test]
        public void Removes()
        {
            a.Include(1, 2);

            a.Exclude(2);

            Assert.AreEqual(1, a.Count);
            Assert.That(a.AsEnumerable(), Is.EquivalentTo(new[] {1}));
        }

        [Test]
        public void Creates()
        {
            b.Include(1, 2, 3, 4);

            Assert.AreEqual(4, b.Count);
        }

        [Test]
        public void HandlesBadInclude()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => a.Include(5));
            Assert.AreEqual(0, a.Count);
        }

        [Test]
        public void Unions()
        {
            a.Include(4);
            b.Include(1, 2);

            var c = Set<int>.Union(a, b);

            Assert.AreEqual(3, c.Count);
            Assert.That(c, Is.EquivalentTo(new[] {1, 2, 4}));
        }

        [Test]
        public void Intersects()
        {
            var b = a.GetEmptySubset();
            b.Include(1, 2);
            a.Include(2);

            var c = Set<int>.Intersect(a, b);

            Assert.AreEqual(1, c.Count);
            Assert.That(c, Is.EquivalentTo(new[] {2}));
        }

        [Test]
        public void Inverts()
        {
            a.Include(1, 2);
            
            var inv = a.Inverted();
            
            Assert.That(inv, Is.EquivalentTo(new []{3, 4}));
        }
        
        [Test]
        public void FoundsSubsets()
        {
            a.Include(1, 2);
            b.Include(1, 2, 3);
           
            Assert.AreEqual(true, a.IsSubsetOf(b));
            
            a.Include(3);
            
            Assert.AreEqual(false, a.IsSubsetOf(b));
        }
    }
}