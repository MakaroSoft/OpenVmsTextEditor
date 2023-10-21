import {parser} from "../lezer-decBasic/parser"
import {LRLanguage, LanguageSupport} from "@codemirror/language"
import {completeFromList, ifNotIn} from "@codemirror/autocomplete"
import {snippets} from "./snippets"
import {localCompletionSource, dontComplete} from "./complete"

export const decBasicLanguage = LRLanguage.define({
  name: "decBasic",
  parser: parser.configure({}),
  languageData: {}
})


let kwCompletion = (name: string) => ({label: name, type: "keyword"})

const upperKeywords = // operator keywords
"IF THEN ELSE END RETURN GOSUB GOTO TYPE REAL SIZE ON ITERATE ERROR OPTION PRINT OPEN FOR INPUT OUTPUT BACK ORGANIZATION FIXED ACCESS MODIFY ALLOW READ INDEXED AS FILE SEQUENTIAL FOR TO SELECT CASE DO WHILE NEXT CLOSE WHEN IN UNTIL USE BREAK STEP " +

  // definition keywords
  "%INCLUDE EXTERNAL MAP DIM DECLARE FUNCTION " +
  
  // primitive types
  "STRING BYTE LONG FLOAT DOUBLE GFLOAT QUAD WORD";

const lowerKeywords = upperKeywords.toLowerCase();
const allKeywords = lowerKeywords + " " + upperKeywords;
const keywords = allKeywords.split(" ").map(kwCompletion)

export function decBasic() {
  let lang = decBasicLanguage
  let completions = snippets.concat(keywords)
  return new LanguageSupport(lang, [
    decBasicLanguage.data.of({
      autocomplete: ifNotIn(dontComplete, completeFromList(completions))
    }),
    decBasicLanguage.data.of({
      autocomplete: localCompletionSource
    }),
    [],
  ])
}
