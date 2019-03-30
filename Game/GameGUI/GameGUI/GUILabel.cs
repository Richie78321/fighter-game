using System;
using System.Collections.Generic;
using GameGUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameGUI
{
    public class GUILabel : GUIElement
    {
        private const float TEXT_PADDING = .25F;
        public const char DEFAULT_CENSORED_CHAR = '*';

        //Object
        private bool censored;
        private char censoredChar;

        private SpriteFont font;
        public SpriteFont Font => font;
        public Color TextColor;
        private string actualText;
        protected string text
        {
            get
            {
                if (censored)
                {
                    string censoredString = "";
                    for (int i = 0; i < actualText.Length; i++) censoredString += DEFAULT_CENSORED_CHAR;
                    return censoredString;
                }
                else return actualText;
            }
        }
        public string Text => actualText;
        private float fontScale;

        public GUILabel(Point centerPosition, string text, SpriteFont font, Color textColor, Rectangle textBounds, bool wordWrap = false)
        {
            this.font = font;
            this.TextColor = textColor;
            this.actualText = text;

            FindScale(centerPosition, textBounds, wordWrap);
        }

        public GUILabel(Point centerPosition, string text, SpriteFont font, Color textColor, Rectangle textBounds, char censoredChar)
        {
            censored = true;
            this.censoredChar = censoredChar;

            this.font = font;
            this.TextColor = textColor;
            this.actualText = text;

            FindScale(centerPosition, textBounds, false);
        }

        private void FindScale(Point centerPosition, Rectangle textBounds, bool wordWrap)
        {
            //Find scale
            if (wordWrap)
            {
                //Get words
                string[] words = text.Split(' ');
                int wordLength = 0;
                for (int i = 0; i < words.Length; i++) wordLength += words[i].Length;

                //Find preferred line
                int lineNum = 0;
                int numBlank = 0;
                string[] splitLines;
                Vector2 measureString;
                float proposedScale = 0;
                do
                {
                    lineNum++;
                    //Apply scale
                    fontScale = proposedScale;

                    //Find new scale
                    List<string> wordList = new List<string>(words);
                    splitLines = new string[lineNum];
                    for (int i = 0; i < splitLines.Length; i++)
                    {
                        int endLength = (int)Math.Ceiling((float)wordLength / splitLines.Length);
                        int endIndex = -1;
                        int wordLengthSum = 0;
                        for (int j = 0; j < wordList.Count; j++)
                        {
                            wordLengthSum += wordList[j].Length;
                            if (wordLengthSum >= endLength)
                            {
                                endIndex = j;
                                break;
                            }
                        }
                        if (endIndex == -1) endIndex = wordList.Count - 1;

                        string splitString = "";
                        for (int j = 0; j <= endIndex; j++) splitString += wordList[j] + " ";

                        splitLines[i] = splitString;
                        wordList.RemoveRange(0, endIndex + 1);
                    }
                    splitLines[splitLines.Length - 1].TrimEnd(' ');

                    int largestLine = 0;
                    for (int i = 1; i < splitLines.Length; i++) if (splitLines[i].Length > splitLines[largestLine].Length) largestLine = i;

                    numBlank = 0;
                    for (int i = 0; i < splitLines.Length; i++) if (string.IsNullOrEmpty(splitLines[i])) numBlank++; 

                    measureString = font.MeasureString(splitLines[largestLine]);
                    proposedScale = Math.Min((textBounds.Width * (1F - TEXT_PADDING)) / measureString.X, ((textBounds.Height * (1F - TEXT_PADDING)) / (measureString.Y * ((2 * (lineNum - numBlank)) - 1))));
                }
                while (proposedScale > fontScale);

                actualText = splitLines[splitLines.Length - 1];
                for (int i = splitLines.Length - 2; i >= 0; i--) actualText = splitLines[i] + "\n\n" + text;
                elementRectangle = new Rectangle(centerPosition.X - (int)((measureString.X * fontScale) / 2f), centerPosition.Y - (int)(((measureString.Y * ((2 * (lineNum - numBlank)) - 1)) * fontScale) / 2f), (int)(measureString.X * fontScale), (int)((measureString.Y * ((2 * lineNum) - 1)) * fontScale));
            }
            else
            {
                Vector2 stringSize = font.MeasureString(text);
                fontScale = (textBounds.Width * (1F - TEXT_PADDING)) / stringSize.X;
                float heightScale = (textBounds.Height * (1F - TEXT_PADDING)) / stringSize.Y;
                if (heightScale < fontScale) fontScale = heightScale;
                elementRectangle = new Rectangle(centerPosition.X - (int)((stringSize.X * fontScale) / 2f), centerPosition.Y - (int)((stringSize.Y * fontScale) / 2f), (int)(stringSize.X * fontScale), (int)(stringSize.Y * fontScale));
            }
        }

        protected override void HandleDraw(SpriteBatch spriteBatch, Rectangle scissorRectangle, DrawSpecifics drawSpecifics)
        {
            Vector4 color = TextColor.ToVector4();
            spriteBatch.DrawString(font, text, Location, new Color(color.X * drawSpecifics.Opacity, color.Y * drawSpecifics.Opacity, color.Z * drawSpecifics.Opacity, color.W * drawSpecifics.Opacity), 0, Vector2.Zero, fontScale, SpriteEffects.None, 0);
        }

        protected override void HandleUpdate(GameTime gameTime)
        {
        }

        protected override void HandleInteract(GUIInput guiInput)
        {
        }
    }
}
