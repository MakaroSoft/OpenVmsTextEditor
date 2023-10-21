import {styleTags, tags as t} from "@lezer/highlight"
import {HighlightStyle} from "@codemirror/language"

export const myHighlightStyle = HighlightStyle.define([
  {tag: t.lineComment, color: "#00800D" },
  {tag: t.blockComment, color: "#00800D" },
  {tag: t.keyword, color: "#A908C4"},
  {tag: t.definitionKeyword, color: "#0000FF"},
  {tag: t.string, color: "#C96B15"}
])


export const basicHighlight = styleTags({
  // operator keywords
  Keyword: t.operatorKeyword,
  DefinitionKeyword: t.definitionKeyword,

  // comments
  LineComment: t.lineComment,

  // primitive types - well they all are in basic :)
  PrimitiveType: t.definitionKeyword,

  FloatingPointLiteral: t.number,
  IntegerLiteral: t.number,
  StringLiteral: t.string
})
