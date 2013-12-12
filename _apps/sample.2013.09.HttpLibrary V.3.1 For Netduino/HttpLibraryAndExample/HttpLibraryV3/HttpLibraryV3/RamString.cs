using System;
using Microsoft.SPOT;

namespace HttpLibrary
{
    /// <summary>
    /// Class for handling ram strings
    /// </summary>
    public class RamString
    {
        private int position;
        private int end_position;
        private RamStream stream;

        /// <summary>
        /// String length
        /// </summary>
        public int Length
        {
            get
            {
                int i = 0;
                for (i = position; ((char)stream.Read(i)) != '\0'; ++i) ;
                return i - position;
            }
        }

        /// <summary>
        /// Ram adress where the string is stored
        /// </summary>
        public int Position { get { return position; } }

        /// <summary>
        /// Class constructor, initializes position to address 0
        /// </summary>
        /// <param name="stream">RamStream object</param>
        public RamString(RamStream stream)
        {
            this.stream = stream;
            this.position = 0;
        }

        /// <summary>
        /// Class constructor, initializes position to a specified address
        /// </summary>
        /// <param name="stream">RamStream object</param>
        /// <param name="address">address to store strings</param>
        public RamString(RamStream stream, int address)
        {
            this.stream = stream;
            this.position = address;
        }

        /// <summary>
        /// Writes a string into ram
        /// </summary>
        /// <param name="value">String to write</param>
        public void Write(string value)
        {
            int loop_counter = value.Length + position;
            for (int i = position, j = 0; i < loop_counter; ++i, ++j)
                stream.Write(i, (byte)value[j]);
            stream.Write(loop_counter, (byte)'\0');
        }

        /// <summary>
        /// Writes a char array into ram
        /// </summary>
        /// <param name="value">char array value</param>
        public void Write(char[] value)
        {
            int loop_counter = value.Length + position;
            for (int i = position, j = 0; i < loop_counter; ++i, ++j)
                stream.Write(i, (byte)value[j]);
            stream.Write(loop_counter, (byte)'\0');
        }

        /// <summary>
        /// Writes a byte array on to ram
        /// </summary>
        /// <param name="value">byte array value</param>
        public void Write(byte[] value)
        {
            int loop_counter = value.Length + position;
            for (int i = position, j = 0; i < loop_counter; ++i, ++j)
                stream.Write(i, value[j]);
            stream.Write(loop_counter, (byte)'\0');
        }

        /// <summary>
        /// Returns the first index of the containing value else -1 (same as string.IndexOf(..))
        /// </summary>
        /// <param name="Containing">Value to search</param>
        /// <returns></returns>
        public int IndexOf(string Containing)
        {
            int original_length = this.Length;
            int containing_length = Containing.Length;
            int loop1, loop2, match;
            bool found = false;
            for (loop1 = position; loop1 < original_length + position; ++loop1)
            {
                for (loop2 = 0, match = 0; loop2 < containing_length; ++loop2)
                {
                    if (((char)stream.Read(loop2 + loop1)) == Containing[loop2])
                    {
                        ++match;
                        if (match == containing_length)
                        {
                            found = true;
                            break;
                        }
                    }
                }
                if (found)
                    return loop1 - position;
                else
                    match = 0;
            }
            return -1;
        }

        /// <summary>
        /// Use this function to write strings with custom length.
        /// Use WriteSingle() with it
        /// Use EndWrite() after finishing
        /// </summary>
        /// <param name="address">Start address</param>
        public void BeginWrite(int address)
        {
            position = address;
            end_position = address;
        }

        /// <summary>
        /// Used with BeginWrite() and EndWrite() to write a byte value
        /// </summary>
        /// <param name="value">Value to write</param>
        public void WriteSingle(byte value)
        {
            stream.Write(end_position++, value);
        }

        /// <summary>
        /// Used with BeginWrite() and EndWrite() to write a char value
        /// </summary>
        /// <param name="value">Value to write</param>
        public void WriteSingle(char value)
        {
            stream.Write(end_position++, (byte)value);
        }

        /// <summary>
        /// Terminates the written string and the string is ready to manipulate
        /// </summary>
        public void EndWrite()
        {
            stream.Write(end_position, (byte)'\0');
        }

        /// <summary>
        /// Prints the string from the ram
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            char[] buff = new char[this.Length];
            int length = buff.Length;
            for (int i = position, j = 0; j < length; ++i, ++j)
                buff[j] = (char)stream.Read(i);
            return new string(buff);
        }

        /// <summary>
        /// Gets the character available specified by the address
        /// </summary>
        /// <param name="index">Address</param>
        /// <returns>Character</returns>
        public char this[int index]
        {
            get
            {
                return ((char)stream.Read(index));
            }
        }
    }
}
