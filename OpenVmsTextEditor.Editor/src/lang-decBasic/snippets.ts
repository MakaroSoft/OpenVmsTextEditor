import {Completion, snippetCompletion as snip} from "@codemirror/autocomplete"

/// A collection of JavaScript-related
/// [snippets](#autocomplete.snippet).
export const snippets: readonly Completion[] = [
  snip("%INCLUDE \"${}\"", {
    label: "%INCLUDE",
    detail: "literal string",
    type: "keyword"
  }),
  snip("%include \"${}\"", {
    label: "%include",
    detail: "literal string",
    type: "keyword"
  }),
  snip("SELECT ${field}\nCASE ${value}\n\t\t${}\nCASE ELSE\n\t\t${}\nEND SELECT", {
    label: "SELECT",
    detail: "case",
    type: "keyword"
  }),
  snip("select ${field}\ncase ${value}\n\t\t${}\ncase else\n\t\t${}\nend select", {
    label: "select",
    detail: "case",
    type: "keyword"
  }),
   snip("ON ERROR GOTO ${line}", {
    label: "ON",
    detail: "ERROR GOTO line",
    type: "keyword"
  }),
  snip("on error goto ${line}", {
    label: "on",
    detail: "error goto line",
    type: "keyword"
  }),
  snip("FOR ${index%} = ${start%} TO ${end%}\n\t\t${}\nNEXT", {
    label: "FOR",
    detail: "loop",
    type: "keyword"
  }),
  snip("for ${index%} = ${start%} to ${end%}\n\t\t${}\nnext", {
    label: "for",
    detail: "loop",
    type: "keyword"
  }),
  snip("FOR ${index%} = ${start%} TO ${end%} STEP ${step%}\n\t\t${}\nNEXT", {
    label: "FOR",
    detail: "loop STEP",
    type: "keyword"
  }),
  snip("for ${index%} = ${start%} to ${end%} step ${step%}\n\t\t${}\nnext", {
    label: "for",
    detail: "loop step",
    type: "keyword"
  }),
  snip("WHEN ERROR IN\n\t\t${}\nUSE\n\t\t ${}\nEND WHEN", {
    label: "WHEN",
    detail: "ERROR IN",
    type: "keyword"
  }),
  snip("when error in\n\t\t${}\nuse\n\t\t ${}\nend when", {
    label: "when",
    detail: "error in",
    type: "keyword"
  }),
  snip("WHILE ${exp1}\n\t\t${}\nNEXT", {
    label: "WHILE",
    detail: "loop",
    type: "keyword"
  }),
  snip("while ${exp1}\n\t\t${}\nnext", {
    label: "while",
    detail: "loop",
    type: "keyword"
  }),
  snip("UNTIL ${exp1}\n\t\t${}\nNEXT", {
    label: "UNTIL",
    detail: "loop",
    type: "keyword"
  }),
  snip("until ${exp1}\n\t\t${}\nnext", {
    label: "until",
    detail: "loop",
    type: "keyword"
  }),
  snip("IF ${exp} THEN\n\t\t${}\nEND IF", {
    label: "IF",
    detail: "block",
    type: "keyword"
  }),
  snip("if ${exp} then\n\t\t${}\nend if", {
    label: "if",
    detail: "block",
    type: "keyword"
  }),
  snip("IF ${exp} THEN\n\t\t${}\nELSE\n\t\t${}\nEND IF", {
    label: "IF",
    detail: "/ ELSE block",
    type: "keyword"
  }),
  snip("if${exp} then\n\t\t${}\nelse\n\t\t${}\nend if", {
    label: "if",
    detail: "/ else block",
    type: "keyword"
  })
]