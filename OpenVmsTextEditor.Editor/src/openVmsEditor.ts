import {basicSetup, EditorView} from "codemirror"
import {keymap} from "@codemirror/view"
import {indentLess} from "@codemirror/commands"
import {Compartment, EditorState, EditorSelection} from "@codemirror/state"
import {decBasic} from "./lang-decBasic/basic"
import {syntaxHighlighting} from "@codemirror/language"
import {myHighlightStyle} from "./lezer-decBasic/highlight.js"


declare global {
  interface Window { onDocumentChanged: any; }
}
const listenerExtension = EditorView.updateListener.of(editorUpdate => {
  if (editorUpdate.docChanged) window.onDocumentChanged();
});

function insertTab(view: EditorView) {
  let changes = view.state.changeByRange(range => {
    if (!range.empty) return { range };
    return {
        changes: { from: range.from, to: range.from, insert: "\t"},
        range: EditorSelection.cursor(range.from + 1)        
    };
  });
  if (changes.changes.empty) return false;
  view.dispatch(view.state.update(changes, { scrollIntoView: true }));
  return true;
}

export function load(text: string, querySelector: string) {

  let tabSize = new Compartment
  
  return new EditorView({
    //state: state,
    doc: text,
    extensions: [
        basicSetup,
        decBasic(),
        syntaxHighlighting(myHighlightStyle),
        listenerExtension,
        tabSize.of(EditorState.tabSize.of(8)),
        keymap.of([
          {key: "Tab", run: insertTab},
          {key: "Shift-Tab", run: indentLess}
        ])
    ],
    parent: <Element>document.querySelector(querySelector)
  });
}