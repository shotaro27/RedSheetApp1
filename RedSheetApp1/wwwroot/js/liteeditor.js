$(() => {
    var btnOptions = [
        {
            label: 'red',
            action: 'extra',
            group: 'mark',
            onClick: (editor) => {
                var node = editor.getSelectionNode();
                if (node.classList.contains("keyword-wrong")) {
                    editor.unwrapTag("span", "keyword-wrong");
                }
                else if (node.classList.contains("keyword-right")) {
                    editor.unwrapTag("span", "keyword-right");
                }
                else {
                    editor.insertTag('span', 'keyword-wrong', '');
                }
                console.log(node);
            }
        }
    ];

    var editor = new LiteEditor('.js-editor', {
        btnOptions: btnOptions,
        source: true
    });
});