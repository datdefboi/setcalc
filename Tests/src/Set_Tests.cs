using System;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Tests
{
    public class Set_Tests
    {
        private Set<int> set;

        [SetUp]
        public void Setup()
        {
            set = new Set<int>(new[] {1, 2, 3, 4});
        }

        [Test]
        public void Counts()
        {
            Assert.AreEqual(0, set.Count);

            set.Include(1, 2);
            Assert.AreEqual(2, set.Count);

            var rp = set.Include(2);
            Assert.AreEqual(1, rp);
            Assert.AreEqual(2, set.Count);
        }

        [Test]
        public void Clears()
        {
            set.Include(1, 2);

            set.Clear();

            Assert.AreEqual(0, set.Count);
            Assert.That(set.AsEnumerable(), Is.Empty);
        }

        [Test]
        public void Removes()
        {
            set.Include(1, 2);

            set.Exclude(2);

            Assert.AreEqual(1, set.Count);
            Assert.That(set.AsEnumerable(), Is.EquivalentTo(new[] {1}));
        }

        [Test]
        public void Creates()
        {
            var b = set.GetEmptySubset();

            b.Include(1, 2, 3, 4);

            Assert.AreEqual(4, b.Count);
        }

        [Test]
        public void HandlesBadInclude()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => set.Include(5));
            Assert.AreEqual(0, set.Count);
        }

        [Test]
        public void Unions()
        {
            var b = set.GetEmptySubset();
            b.Include(1, 2);
            set.Include(4);

            set.UnionWith(b);

            Assert.AreEqual(3, set.Count);
            Assert.That(set, Is.EquivalentTo(new[] {1, 2, 4}));
        }

        [Test]
        public void Intersects()
        {
            var b = set.GetEmptySubset();
            b.Include(1, 2);
            set.Include(2);

            set.IntersectWith(b);

            Assert.AreEqual(1, set.Count);
            Assert.That(set, Is.EquivalentTo(new[] {2}));
        }

        [Test]
        public void Inverts()
        {
            set.Include(1, 2);
            
            set.Invert();
            
            Assert.That(set, Is.EquivalentTo(new []{3, 4}));
        }
    }
}