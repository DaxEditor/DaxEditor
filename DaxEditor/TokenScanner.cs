// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using Babel.Parser;

namespace DaxEditor
{
    public class TokenLocation
    {
        public Tokens Token { get; set; }
        public int Location { get; set; }
        public int Length { get; set; }
    }

    public class TokenScanner : IEnumerable<TokenLocation>
    {
        class TokenScannerEnumerator : IEnumerator<TokenLocation>, IDisposable
        {
            Babel.ParserGenerator.IColorScan _lex = null;
            int _state, _startIndex, _endIndex;
            TokenLocation _currentTokenLocation = null;
            Func<TokenLocation, bool> _skipFilter;
            readonly string _source;
            readonly int _startOffset;

            public TokenScannerEnumerator(string source, int offset, Func<TokenLocation, bool> skipFilter)
            {
                _source = source;
                _startOffset = offset;
                _skipFilter = skipFilter;
                Reset();
            }

            public TokenLocation Current
            {
                get { return _currentTokenLocation; }
            }

            public void Dispose()
            {
                Dispose(true);
            }

            private void Dispose(bool fromDispose)
            {
                GC.SuppressFinalize(this);
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                _currentTokenLocation = new TokenLocation()
                    {
                        Token = (Tokens) _lex.GetNext(ref _state, out _startIndex, out _endIndex),
                        Location = _startIndex + _startOffset,
                        Length = _endIndex - _startIndex + 1,
                    };

                if (_skipFilter != null && _skipFilter(_currentTokenLocation))
                    return MoveNext();

                return _currentTokenLocation.Token != Tokens.EOF;
            }

            public void Reset()
            {
                _lex = new Babel.Lexer.Scanner();
                _lex.SetSource(_source, 0);
                _startIndex = _endIndex = 0;
                _currentTokenLocation = null;
            }
        }

        /// <summary>
        /// String to scan
        /// </summary>
        readonly string _source;

        /// <summary>
        /// Start offset in the string to scan
        /// </summary>
        readonly int _startOffset;

        /// <summary>
        /// Filter for skipping token locations
        /// </summary>
        Func<TokenLocation, bool> _skipFilter;

        public TokenScanner(string source, int offset, Func<TokenLocation, bool> skipFilter = null)
        {
            _source = source;
            _startOffset = offset;
            _skipFilter = skipFilter;
        }

        public IEnumerator<TokenLocation> GetEnumerator()
        {
            return new TokenScannerEnumerator(_source, _startOffset, _skipFilter);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
