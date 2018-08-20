/*
MIT License

Copyright(c) 2018 Gratian Pawliszyn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.IO;

namespace Polynano.Persistence.PolygonFileFormat.Readers.StreamReaders
{
    /// <summary>
    /// The loading performence was not satisfactory
    /// so that a custom stream reader was developed.
    /// It is about 40% faster than the standard StreamReader although 
    /// some compromises had to be made. 
    /// Also this allows us to accept non-seekable streams
    /// as this reader can read ascii and then instantly switch to binary.
    /// </summary>
    public class BufferedStreamReader : ITextReader, IBinaryReader, IDisposable
    {
        private readonly Stream _source;

        private readonly long _totalStreamLength;

        private readonly byte[] _buffer;

        private int _realBufferLength;

        private int _positionInBuffer;

        private bool _disposeSource;

        public BufferedStreamReader(Stream source, int bufferSize = 4096, bool disposeStream = false)
        {
            if (bufferSize < 0 || bufferSize > 4096)
                  throw new ArgumentOutOfRangeException(nameof(bufferSize));

            if (!source.CanRead)
                throw new ArgumentException("Stream must be writeable");

            _source = source;
            _disposeSource = disposeStream;
            _buffer = new byte[bufferSize];
            _realBufferLength = bufferSize;
            _totalStreamLength = source.Length;
            _positionInBuffer = -1;
        }

        private bool EndOfStream => _source.Position == _totalStreamLength;

        private char CurrentChar => (char)_buffer[_positionInBuffer];

        private bool EndOfBuffer => (_positionInBuffer == -1 || _positionInBuffer >= _realBufferLength);

        // Note that if the line contains whitespaces on the beginning or end,
        // it will be trimmed.
        public string ReadLine()
        {
            return ReadUntilBlank(spaceTerminates: false);
        }

        public string ReadToken()
        {
            return ReadUntilBlank(spaceTerminates: true);
        }

        private string ReadUntilBlank(bool spaceTerminates)
        {
            if (EndOfStream && EndOfBuffer)
                return null;

            if (EndOfBuffer)
                UpdateBuffer();

            // Jump to the first non whitespace and non-newline
            // character, go through multiple buffers if needed.
            JumpFirstNotBlank();

            // set the space to \0 character if the client
            // does not want the space to terminate.
            char space = spaceTerminates ? ' ' : char.MinValue;

            // Remember the token begin position
            // and move the bufferPosition to the end of this token.
            int begin = _positionInBuffer;
            while (_positionInBuffer < _realBufferLength
                && CurrentChar != '\n'
                && CurrentChar != '\r'
                && CurrentChar != space)
            {
                _positionInBuffer++;
            }

            // If we've reached the end of the buffer before 
            // reaching the end of the token, we need to update the buffer
            // and combine the two read tokens

            string str;
            if (EndOfBuffer
                && (char)_buffer[_positionInBuffer - 1] != '\n'
                && (char)_buffer[_positionInBuffer - 1] != '\r'
                && (char)_buffer[_positionInBuffer - 1] != space)
            {
                string temp = ReadBufferAsASCII(begin, _positionInBuffer - begin);

                while (!EndOfStream)
                {
                    UpdateBuffer();
                    while (_positionInBuffer < _realBufferLength
                           && CurrentChar != '\n'
                           && CurrentChar != '\r'
                           && CurrentChar != space)
                    {
                        _positionInBuffer++;
                    }
                    temp += ReadBufferAsASCII(0, _positionInBuffer);
                    if (!EndOfBuffer)
                        break;
                }
                str = temp;
            }
            else
            {
                str = ReadBufferAsASCII(begin, _positionInBuffer - begin);
            }

            // We need to make sure we're not leaving the buffer on whitespace
            // should the binary read follow.
            JumpFirstNotBlank();

            return str;
        }

        public byte[] ReadBytes(int count)
        {
            if(count > _realBufferLength)
                throw new ArgumentOutOfRangeException(nameof(count));
            
            if (EndOfStream && EndOfBuffer)
                return null;

            if (EndOfBuffer)
                UpdateBuffer();

            byte[] val = new byte[count];
            // If the count is bigger than the unread part of the buffer
            // we need to consume the current buffer, load next part and combine them.
            // this only supports one buffer update. So that it is neccessary to throw ArgumentOutOfRange
            // if the requested size is too big. There's not really a need to implement multiple buffer updates.
            if(count > _realBufferLength - _positionInBuffer)
            {
                Array.Copy(_buffer, _positionInBuffer, val, 0, _realBufferLength - _positionInBuffer);
                int remaining = count - (_realBufferLength - _positionInBuffer);
                UpdateBuffer();
                Array.Copy(_buffer, _positionInBuffer, val, count - remaining, remaining);
                _positionInBuffer += remaining;
            }
            else
            {
                Array.Copy(_buffer, _positionInBuffer, val, 0, count);
                _positionInBuffer += count;
            }

            return val;
        }

        private void UpdateBuffer()
        {
            long sizeToRead = _buffer.Length;

            if (_totalStreamLength - _source.Position <= sizeToRead)
            {
                sizeToRead = _source.Length - _source.Position;
            }

            _source.Read(_buffer, 0, (int)sizeToRead);
            _realBufferLength = (int)sizeToRead;
            _positionInBuffer = 0;
        }

        private void JumpFirstNotBlank()
        {
            if (EndOfBuffer && EndOfStream)
                return;

            while (CurrentChar == ' '
                    || CurrentChar == '\r'
                    || CurrentChar == '\n')
            {
                _positionInBuffer++;
                if (EndOfBuffer)
                {
                    if (!EndOfStream)
                        UpdateBuffer();
                    else
                        break;
                }
            }
        }

        private string ReadBufferAsASCII(int startIndex, int length)
        {
            //  Do not use Encoding.Default.GetString(_buffer, startIndex, length);
            //  as it's slow.
            var tmp = new char[length];
            for (int i = 0; i < length; ++i)
                tmp[i] = (char)_buffer[startIndex + i];
            var str = new string(tmp);
            return str;
        }

        public void Dispose()
        {
            if (_disposeSource)
                _source?.Dispose();
        }
    }
}
