using System;
using System.IO;
using System.Text;

/* Originally written in 'C', this code has been converted to the C# language.
 * The author's copyright message is reproduced below.
 * All modifications from the original to C# are placed in the public domain.
 */

/* jsmin.c
   2007-05-22

Copyright (c) 2002 Douglas Crockford  (www.crockford.com)

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

The Software shall be used for Good, not Evil.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace TeamMentor.CoreLib
{



	public class JavaScriptMinifier
	{
		private const int EOF = -1;

		private StreamReader _sr;
		private StreamWriter _sw;
		private int _theA;
		private int _theB;
		private int _theLookahead = EOF;

		public string Minify(string src)
		{
			var srcStream = new MemoryStream(Encoding.Unicode.GetBytes(src));
			var tgStream = new MemoryStream(8092);

			using (_sr = new StreamReader(srcStream, Encoding.Unicode))
			{
				using (_sw = new StreamWriter(tgStream, Encoding.Unicode))
				{
					jsmin();
				}
			}

			return Encoding.Unicode.GetString(tgStream.ToArray());
		}

		/* jsmin -- Copy the input to the output, deleting the characters which are
				insignificant to JavaScript. Comments will be removed. Tabs will be
				replaced with spaces. Carriage returns will be replaced with linefeeds.
				Most spaces and linefeeds will be removed.
		*/

		private void jsmin()
		{
			_theA = '\n';
			action(3);
			while (_theA != EOF)
			{
				switch (_theA)
				{
					case ' ':
				        {
				            action(isAlphanum(_theB) ? 1 : 2);
				            break;
				        }
				    case '\n':
						{
							switch (_theB)
							{
								case '{':
								case '[':
								case '(':
								case '+':
								case '-':
									{
										action(1);
										break;
									}
								case ' ':
									{
										action(3);
										break;
									}
								default:
							        {
							            action(isAlphanum(_theB) ? 1 : 2);
							            break;
							        }
							}
							break;
						}
					default:
						{
							switch (_theB)
							{
								case ' ':
									{
										if (isAlphanum(_theA))
										{
											action(1);
											break;
										}
										action(3);
										break;
									}
								case '\n':
									{
										switch (_theA)
										{
											case '}':
											case ']':
											case ')':
											case '+':
											case '-':
											case '"':
											case '\'':
												{
													action(1);
													break;
												}
											default:
										        {
										            action(isAlphanum(_theA) ? 1 : 3);
										            break;
										        }
										}
										break;
									}
								default:
									{
										action(1);
										break;
									}
							}
							break;
						}
				}
			}
		}

		/* action -- do something! What you do is determined by the argument:
				1   Output A. Copy B to A. Get the next B.
				2   Copy B to A. Get the next B. (Delete A).
				3   Get the next B. (Delete B).
		   action treats a string as a single character. Wow!
		   action recognizes a regular expression if it is preceded by ( or , or =.
		*/

		private void action(int d)
		{
			if (d <= 1)
			{
				put(_theA);
			}
			if (d <= 2)
			{
				_theA = _theB;
				if (_theA == '\'' || _theA == '"')
				{
					for (;;)
					{
						put(_theA);
						_theA = get();
						if (_theA == _theB)
						{
							break;
						}
						if (_theA <= '\n')
						{
							throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}\n", _theA));
						}
						if (_theA == '\\')
						{
							put(_theA);
							_theA = get();
						}
					}
				}
			}
			if (d <= 3)
			{
				_theB = next();
				if (_theB == '/' && (_theA == '(' || _theA == ',' || _theA == '=' ||
				                    _theA == '[' || _theA == '!' || _theA == ':' ||
				                    _theA == '&' || _theA == '|' || _theA == '?' ||
				                    _theA == '{' || _theA == '}' || _theA == ';' ||
				                    _theA == '\n'))
				{
					put(_theA);
					put(_theB);
					for (;;)
					{
						_theA = get();
						if (_theA == '/')
						{
							break;
						}
						if (_theA == '\\')
						{
							put(_theA);
							_theA = get();
						}
						else if (_theA <= '\n')
						{
							throw new Exception(string.Format("Error: JSMIN unterminated Regular Expression literal : {0}.\n", _theA));
						}
						put(_theA);
					}
					_theB = next();
				}
			}
		}

		/* next -- get the next character, excluding comments. peek() is used to see
				if a '/' is followed by a '/' or '*'.
		*/

		private int next()
		{
			int c = get();
			if (c == '/')
			{
				switch (peek())
				{
					case '/':
						{
							for (;;)
							{
								c = get();
								if (c <= '\n')
								{
									return c;
								}
							}
						}
					case '*':
						{
							get();
							for (;;)
							{
								switch (get())
								{
									case '*':
										{
											if (peek() == '/')
											{
												get();
												return ' ';
											}
											break;
										}
									case EOF:
										{
											throw new Exception("Error: JSMIN Unterminated comment.\n");
										}
								}
							}
						}
					default:
						{
							return c;
						}
				}
			}
			return c;
		}

		/* peek -- get the next character without getting it.
		*/

		private int peek()
		{
			_theLookahead = get();
			return _theLookahead;
		}

		/* get -- return the next character from stdin. Watch out for lookahead. If
				the character is a control character, translate it to a space or
				linefeed.
		*/

		private int get()
		{
			int c = _theLookahead;
			_theLookahead = EOF;
			if (c == EOF)
			{
				c = _sr.Read();
			}
			if (c >= ' ' || c == '\n' || c == EOF)
			{
				return c;
			}
			if (c == '\r')
			{
				return '\n';
			}
			return ' ';
		}

		private void put(int c)
		{
			_sw.Write((char) c);
		}

		/* isAlphanum -- return true if the character is a letter, digit, underscore,
				dollar sign, or non-ASCII character.
		*/

		private bool isAlphanum(int c)
		{
			return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
			        (c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' ||
			        c > 126);
		}
	}
}