$(() => {
    var btnOptions = [
        {
            label: 'マークを付ける/外す',
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
                else if (node.classList.contains("lite-editor") && window.getSelection().toString().length != 0) {
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

    $("#content-area").contextmenu(e => {
        var node = editor.getSelectionNode();
        if (node.classList.contains("keyword-wrong")) {
            editor.unwrapTag("span", "keyword-wrong");
        }
        else if (node.classList.contains("keyword-right")) {
            editor.unwrapTag("span", "keyword-right");
        }
        else if (node.classList.contains("lite-editor") && window.getSelection().toString().length != 0) {
            editor.insertTag('span', 'keyword-wrong', '');
        }
        console.log(node);
        return false;
    });
});