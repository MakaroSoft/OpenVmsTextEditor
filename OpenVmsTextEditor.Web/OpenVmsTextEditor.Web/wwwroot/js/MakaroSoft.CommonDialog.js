//Defines the MakaroSoft.CommonDialog class

// create the namespace
if (MakaroSoft === undefined) {
    var MakaroSoft = {};
}

// define the class
MakaroSoft.CommonDialog = function(options) {
    // these are the default options
    var settings = {
        required: false
    };

    // merge options into settings
    jQuery.extend(settings, options);

    var modalId = guid();
    var buttonsId = guid();
    var titleId = guid();
    var messageId = guid();

    var closeButton = settings.required ? "" : '<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>';
    var staticBackdrop = settings.required ? 'data-bs-backdrop="static"  data-bs-keyboard="false"' : "";

    var modalString =
        `<div id="${modalId}" class="modal" ${staticBackdrop} tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 id ="${titleId}" class="modal-title"></h5>
                ${closeButton}
            </div>
            <div class="modal-body">
                <span id="${messageId}"></span>
            </div>
            <div class="modal-footer" id="${buttonsId}">
            </div>
        </div>
    </div>
</div>`;
    var html = $.parseHTML(modalString);
    $("body").append(html);

    var myModalEl = document.getElementById(modalId);
    var modal = window.bootstrap.Modal.getOrCreateInstance(myModalEl);
    var $title = $("#" + titleId);
    var $message = $("#" + messageId);
    var $buttons = $("#" + buttonsId);
    var uniqueId = guid();
    var myCallback = null;

    $buttons.on("click", "." + uniqueId, function () {
        myCallback($(this).text());
    });

    this.show = function (title, message, buttons, callback) {


        myCallback = callback;

        $title.text(title);
        $message.html(message);

        $buttons.empty();

        var lastButton = buttons.slice(-1);
        buttons.forEach((x) => {
            var className = x == lastButton ? "btn-primary" : "btn-secondary";
            var buttonString = `<button type="button" class="btn ${className} ${uniqueId}">${x}</button>`;
            var buttonHtml = $.parseHTML(buttonString);
            $buttons.append(buttonHtml);
        });

        modal.show();
    }

    this.close = function() {
        modal.hide();
    }

    function guid() {
        return "10000000-1000-4000-8000-100000000000".replace(/[018]/g, c =>
            (+c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> +c / 4).toString(16)
        );
    }
}

