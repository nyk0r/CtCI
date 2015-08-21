using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace CtCI {
    [TestFixture]
    public class Ch1_ArraysAndStrings
    {
        #region Test Unique
        public static bool IsUnique1(string txt) {
            if (txt == null) {
                throw new ArgumentNullException("txt");
            }
            
            // if hashing algorithm is good the complexity is linear O(n)
            var set = new HashSet<char>();
            foreach (var ch in txt) {
                if (set.Contains(ch)) {
                    return false;
                }
                set.Add(ch);
            }
            return true;
        }

        public static bool IsUnique2(string txt) {
            if (txt == null) {
                throw new ArgumentNullException("txt");
            }

            // the complexity is O(n^2)
            // 0 + 1 + 2 + .. + n - 1 = (0+(n-1))*n/2 = (n-1)*n/2 
            for (var i = 0; i < txt.Length; i++) {
                for (var j = 0; j < i; j++) {
                    if (txt[i].Equals(txt[j])) {
                        return false;
                    }
                }
            }
            return true;
        }

        [Test]
        public void TestIsUnique() {
            var fixtures = new[] {
                Tuple.Create("", true),
                Tuple.Create("a", true),
                Tuple.Create("aa", false),

                Tuple.Create("abcdefghijklmopqrstuvwxyz", true),
                Tuple.Create("abcdefghigklmopqarstuvwxyz", false),
                Tuple.Create("abcdefghigklmopqarstzuvwxyz", false)
            };

            Assert.Throws<ArgumentNullException>(() => { IsUnique1(null); }, "txt");
            Assert.Throws<ArgumentNullException>(() => { IsUnique2(null); }, "txt");

            foreach (var fixture in fixtures) {
                Assert.AreEqual(fixture.Item2, IsUnique1(fixture.Item1));
                Assert.AreEqual(fixture.Item2, IsUnique2(fixture.Item1));
            }
        }
        #endregion

        #region Check Permutation
        public static bool CheckPermutation(string txt1, string txt2) {
            if (txt1 == null) {
                throw new ArgumentNullException("txt1");
            }
            if (txt2 == null) {
                throw new ArgumentNullException("txt2");
            }

            // if hashing algorithm is good the complexity is O(3*n) = O(n)
            var hash = new Dictionary<char, int>();
            if (txt1.Length != txt2.Length) {
                return false;
            }
            foreach (var ch in txt1) {
                if (hash.ContainsKey(ch)) {
                    hash[ch] = hash[ch] + 1;
                } else {
                    hash[ch] = 1;
                }
            }
            foreach (var ch in txt2) {
                if (!hash.ContainsKey(ch)) {
                    return false;
                }
                hash[ch] = hash[ch] - 1;
                if (hash[ch] < 0) {
                    return false;
                }
            }
            return true;
        }

        [Test]
        public void TestCheckPermutation() {
            var fixtures = new Tuple<Tuple<string, string>, bool>[] {
                Tuple.Create(Tuple.Create("", ""), true),
                Tuple.Create(Tuple.Create("a", ""), false),
                Tuple.Create(Tuple.Create("", "0"), false),

                Tuple.Create(Tuple.Create("abcde", "ebcda"), true),
                Tuple.Create(Tuple.Create("aabcde", "abbcde"), false),
                Tuple.Create(Tuple.Create("abcde", "abcdf"), false)
            };

            Assert.Throws<ArgumentNullException>(() => { CheckPermutation(null, null); }, "txt1");
            Assert.Throws<ArgumentNullException>(() => { CheckPermutation("", null); }, "txt2");

            foreach (var fixture in fixtures) {
                var args = fixture.Item1;
                var result = fixture.Item2;
                Assert.AreEqual(result, CheckPermutation(args.Item1, args.Item2));
            }
        } 
        #endregion

        #region URLify
        public static void Urlify(char[] text, int len) {
            // no checks
            // complexity O(2n) = O(n)
            var spaceCount = 0;
            for (var idx = 0; idx < len; idx++) {
                if (text[idx] == ' ') {
                    spaceCount++;
                }
            }
            var dest = len + 2*spaceCount - 1;
            for (var idx = len - 1; idx >= 0; idx--) {
                if (text[idx] == ' ') {
                    text[dest] = '0'; text[dest - 1] = '2'; text[dest - 2] = '%';
                    dest -= 3;
                } else {
                    text[dest] = text[idx];
                    dest--;
                }
            }
        }

        [Test]
        public void TestUrlify() {
            var fixtures = new[] {
                Tuple.Create(Tuple.Create("", 0), ""),
                Tuple.Create(Tuple.Create("a", 1), "a"),
                Tuple.Create(Tuple.Create(" a  ", 2), "%20a"),
                Tuple.Create(Tuple.Create("a   ", 2), "a%20"),
                Tuple.Create(Tuple.Create(" aa  ", 3), "%20aa"),
                Tuple.Create(Tuple.Create("aa   ", 3), "aa%20"),
                Tuple.Create(Tuple.Create("  a    ", 3), "%20%20a"),
                Tuple.Create(Tuple.Create("a      ", 3), "a%20%20"),
                Tuple.Create(Tuple.Create("Mr John Smith    ", 13), "Mr%20John%20Smith")
            };

            foreach (var fixture in fixtures) {
                var args = fixture.Item1;
                char[] text = args.Item1.ToArray();
                int len = args.Item2;
                char[] result = fixture.Item2.ToArray();

                Urlify(text, len);
                CollectionAssert.AreEqual(result, text);
            }
        }
        #endregion

        #region Palindrome Permutation
        public static bool IsPalindromePermutation(string txt) {
            txt = txt ?? string.Empty;
            txt = Regex.Replace(txt, @"\s", "").ToLower();

            var hash = new Dictionary<char, int>();
            foreach (var ch in txt) {
                if (hash.ContainsKey(ch)) {
                    hash[ch] = hash[ch] + 1;
                } else {
                    hash[ch] = 1;
                }
            }

            var oddFound = false;
            foreach (var value in hash.Values) {
                if (value%2 != 0) {
                    if (oddFound || txt.Length%2 == 0) {
                        return false;
                    }
                    oddFound = true;
                }
            }
            return true;
        }

        [Test]
        public void TestArePalindromePermutations() {
            var fixtures = new Tuple<string, bool>[] {
                Tuple.Create("", true),
                Tuple.Create("    ", true),
                Tuple.Create("  a ", true),
                Tuple.Create("a a", true),
                Tuple.Create("ab", false),
                Tuple.Create("abc", false),
                Tuple.Create("aabbcc", true),
                Tuple.Create("aba", true),
                Tuple.Create("aabcc", true),
                Tuple.Create("aabbbcc", true),
                Tuple.Create("Tact Coa", true)
            };

            foreach (var fixture in fixtures) {
                Assert.AreEqual(fixture.Item2, IsPalindromePermutation(fixture.Item1));
            }
        }
        #endregion

        #region One Way
        public static bool CheckOneEdit(string txt1, string txt2) {
            if (txt1 == null || txt2 == null) {
                throw new ArgumentNullException();
            }

            if (txt1 == txt2) {
                return false;
            }

            if (txt1.Length > txt2.Length) {
                var tmp = txt1;
                txt1 = txt2;
                txt2 = tmp;
            }

            if (txt2.Length - txt1.Length > 1) {
                return false;
            }

            int idx1 = 0, idx2 = 0;
            var found = false;
            while (idx2 < txt1.Length && idx2 < txt2.Length) {
                if (txt1[idx1] == txt2[idx2]) {
                    idx1++;
                } else {
                    if (found) {
                        return false;
                    }
                    found = true;
                    if (txt1.Length == txt2.Length) {
                        idx1++;
                    }
                }
                idx2++;
            }
            return true;
        }

        [Test]
        public void TestCheckOneEdit() {
            var fixtures = new[] {
                Tuple.Create(Tuple.Create("", ""), false),
                Tuple.Create(Tuple.Create("a", "a"), false),
                Tuple.Create(Tuple.Create("abc", "abc"), false),

                Tuple.Create(Tuple.Create("ale", "pale"), true),
                Tuple.Create(Tuple.Create("ale", "alep"), true),
                Tuple.Create(Tuple.Create("ale", "aple"), true),
                Tuple.Create(Tuple.Create("ale", "alpe"), true),
                Tuple.Create(Tuple.Create("bale", "pale"), true),

                Tuple.Create(Tuple.Create("ale", "pole"), false),
                Tuple.Create(Tuple.Create("ale", "ales"), true),
                Tuple.Create(Tuple.Create("ale", "apes"), false),
                Tuple.Create(Tuple.Create("ale", "app"), false),
                Tuple.Create(Tuple.Create("bale", "bate"), true),

                Tuple.Create(Tuple.Create("pale", "ple"), true),
                Tuple.Create(Tuple.Create("pales", "pale"), true),
                Tuple.Create(Tuple.Create("pale", "bale"), true),
                Tuple.Create(Tuple.Create("pale", "bake"), false)
            };

            Assert.Throws<ArgumentNullException>(() => { CheckOneEdit("", null); });
            Assert.Throws<ArgumentNullException>(() => { CheckOneEdit(null, null); });

            foreach (var fixture in fixtures) {
                var args = fixture.Item1;
                var result = fixture.Item2;
                Assert.AreEqual(result, CheckOneEdit(args.Item1, args.Item2));
            }
        }
        #endregion

        #region String Compression
        public string CompressString(string txt) {
            if (txt == null) {
                throw new ArgumentNullException("txt");
            }

            if (txt.Length <= 1) {
                return txt;
            }

            var builder = new StringBuilder(txt.Length/2);
            var current = txt[0];
            var counter = 0;
            foreach (var ch in txt) {
                if (current != ch) {
                    builder.Append(string.Format("{0}{1}", current, counter));
                    current = ch;
                    counter = 1;
                } else {
                    counter++;
                }
            }
            builder.Append(string.Format("{0}{1}", current, counter));
            return builder.Length < txt.Length 
                ? builder.ToString()
                : txt;
        }

        [Test]
        public void TestCompressString() {
            var fixtures = new Tuple<string, string>[] {
                Tuple.Create("", ""),
                Tuple.Create("a", "a"),
                Tuple.Create("abc", "abc"),
                Tuple.Create("aabbcc", "aabbcc"),
                Tuple.Create("abccccc", "a1b1c5"),

                Tuple.Create("aabcccccaaa", "a2b1c5a3")
            };
            foreach (var fixture in fixtures) {
                Assert.AreEqual(fixture.Item2, CompressString(fixture.Item1));
            }
        }
        #endregion

        #region Rotate Matrix
        public static UInt32[,] RotateMatix(UInt32[,] matrix) {
            if (matrix == null) {
                throw new ArgumentNullException("matrix");
            }

            int n = matrix.GetLength(0), m = matrix.GetLength(1);
            var result = new UInt32[m, n];
            for (var i = 0; i < m; i++) {
                for (var j = 0; j < n; j++) {
                    result[i, j] = matrix[j, m - i - 1];
                }
            }
            return result;
        }

        [Test]
        public void TestRotateMatrix() {
            var fixtures = new[] {
                Tuple.Create(new UInt32[0, 0], new UInt32[0, 0]),
                Tuple.Create(new[,] { { 11U } }, new[,] { { 11U } }),

                Tuple.Create(new[,] { { 11U, 12U, 13U } }, new[,] { { 13U }, { 12U }, { 11U } }),
                Tuple.Create(new[,] { { 13U }, { 12U }, { 11U } }, new[,] { { 13U, 12U, 11U } }),

                Tuple.Create(
                    new[,] { { 11U, 12U, 13U }, { 21U, 22U, 23U } },
                    new[,] { { 13U, 23U }, { 12U, 22U }, { 11U, 21U } }),
                Tuple.Create(
                    new[,] { { 13U, 23U }, { 12U, 22U }, { 11U, 21U } },
                    new[,] { { 23U, 22U, 21U }, { 13U, 12U, 11U } }),

                Tuple.Create(
                    new[,] { { 11U, 12U, 13U }, { 21U, 22U, 23U }, { 31U, 32U, 33U } },
                    new[,] { { 13U, 23U, 33U }, { 12U, 22U, 32U }, { 11U, 21U, 31U } }),
                Tuple.Create(
                    new[,] { { 13U, 23U, 33U }, { 12U, 22U, 32U }, { 11U, 21U, 31U } },
                    new[,] { { 33U, 32U, 31U }, { 23U, 22U, 21U, }, { 13U, 12U, 11U } })
            };

            Assert.Throws<ArgumentNullException>(() => { RotateMatix(null); }, "matrix");

            foreach (var fixture in fixtures) {
                CollectionAssert.AreEqual(fixture.Item2, RotateMatix(fixture.Item1));
            }
        }
        #endregion

        #region Zero Matrix
        public static void ZeroMatrixRowAndCol(int[,] matrix) {
            if (matrix == null) {
                throw new ArgumentNullException("matrix");
            }
            int m = matrix.GetLength(0), n = matrix.GetLength(1);
            HashSet<int> rows = new HashSet<int>(), cols = new HashSet<int>();
            for (var i = 0; i < m; i++) {
                for (int j = 0; j < n; j++) {
                    if (matrix[i, j] == 0) {
                        rows.Add(i);
                        cols.Add(j);
                    }
                }
            }
            foreach (var row in rows) {
                for (var k = 0; k < n; k++) {
                    matrix[row, k] = 0;
                }
            }
            foreach (var col in cols) {
                for (int k = 0; k < m; k++) {
                    matrix[k, col] = 0;
                }
            }
        }

        [Test]
        public void TestZeroMatrixRowAndCol() {
            var fixtures = new[] {
                Tuple.Create(new int[0, 0], new int[0, 0]),
                Tuple.Create(new[,] { { 1 } }, new[,] { { 1 } }),
                Tuple.Create(new[,] { { 0 } }, new[,] { { 0 } }),

                Tuple.Create(new[,] { { 1, 1, 1 } }, new[,] { { 1, 1, 1 } }),
                Tuple.Create(new[,] { { 0, 1, 1 } }, new[,] { { 0, 0, 0 } }),
                Tuple.Create(new[,] { { 1, 0, 1 } }, new[,] { { 0, 0, 0 } }),
                Tuple.Create(new[,] { { 1, 1, 0 } }, new[,] { { 0, 0, 0 } }),

                Tuple.Create(new[,] { { 1 }, { 1 }, { 1 } }, new[,] { { 1 }, { 1 }, { 1 } }),
                Tuple.Create(new[,] { { 0 }, { 1 }, { 1 } }, new[,] { { 0 }, { 0 }, { 0 } }),
                Tuple.Create(new[,] { { 1 }, { 0 }, { 1 } }, new[,] { { 0 }, { 0 }, { 0 } }),
                Tuple.Create(new[,] { { 1 }, { 1 }, { 0 } }, new[,] { { 0 }, { 0 }, { 0 } }),

                Tuple.Create(
                    new[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } },
                    new[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } }),
                Tuple.Create(
                    new[,] { { 0, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } },
                    new[,] { { 0, 0, 0 }, { 0, 1, 1 }, { 0, 1, 1 } }),
                Tuple.Create(
                    new[,] { { 1, 1, 1 }, { 1, 0, 1 }, { 1, 1, 1 } },
                    new[,] { { 1, 0, 1 }, { 0, 0, 0 }, { 1, 0, 1 } }),
                Tuple.Create(
                    new[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 0 } },
                    new[,] { { 1, 1, 0 }, { 1, 1, 0 }, { 0, 0, 0 } })
            };

            Assert.Throws<ArgumentNullException>(() => { ZeroMatrixRowAndCol(null); }, "matrix");

            foreach (var fixture in fixtures) {
                ZeroMatrixRowAndCol(fixture.Item1);
                CollectionAssert.AreEqual(fixture.Item2, fixture.Item1);
            }
        }
        #endregion

        #region String Rotation
        public static bool IsStringRotation(string source, string dest) {
            if (source == null || dest == null) {
                throw new ArgumentNullException();
            }

            // IndexOf equivalent to isSubstring
            return source.Length == dest.Length && (source + source).IndexOf(dest, StringComparison.Ordinal) != -1;
        }

        [Test]
        public void TestIsStringRotation() {
            var fixtures = new Tuple<Tuple<string, string>, bool>[] {
                Tuple.Create(Tuple.Create("", ""), true),
                Tuple.Create(Tuple.Create("aa", "aa"), true),
                Tuple.Create(Tuple.Create("aaabc", "aa"), false),
                Tuple.Create(Tuple.Create("aabccc", "ccbaa"), false),
                Tuple.Create(Tuple.Create("xabccc", "abcccx"), true),
                Tuple.Create(Tuple.Create("erbottlewat", "waterbottle"), true)
            };

            Assert.Throws<ArgumentNullException>(() => IsStringRotation(null, ""));
            Assert.Throws<ArgumentNullException>(() => IsStringRotation("", null));

            foreach (var fixture in fixtures) {
                var args = fixture.Item1;
                var result = fixture.Item2;
                Assert.AreEqual(result, IsStringRotation(args.Item1, args.Item2));
            }
        }
        #endregion
    }
}
