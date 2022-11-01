$(function () {
    // localize data-datetime items
    $('[data-datetime]').each(function () {
        let $me = $(this);
        $me.text(datetimeFormatter($me.data('datetime')));
    });

    $('[data-date]').each(function() {
        let $me = $(this);
        $me.text(dateFormatter($me.data('date')));
    });

    $('[data-date-range]').each(function () {
        let $me = $(this);
        $me.text(dateRangeFormatter($me.data('dateRange')));
    });

    var $d = $(document);

    // make modal toggles clickable
    $d.on('click', ".btn-modal", function (e) {
        let btn = $(this);
        let url = btn.data('url');
        let target = $(btn.data('target'));

        $.get(url).done(function (data) {
            target.find(".modal-dialog").html(data);
            target.modal("show");
        });
    });

});


// Wait Icon (uses fontawesome)
jQuery.fn.extend({
    showWaitIcon: function () {
        this.prepend('<i class="fa fa-spinner fa-spin"></i>');
        this.attr('disabled', true);
    },

    hideWaitIcon: function () {
        this.find(':first-child').remove();
        this.attr('disabled', false);
    }
});

// AJAX and toggleable Form functionality

jQuery.fn.extend({

    initAsAjaxModal: function (formHandler) {
        let $form = this;

        if (typeof formHandler === 'undefined') {
            formHandler = {};
        }

        if (typeof formHandler.success !== 'function')
            formHandler.success = function (data, status, xhr) {
                let $modal = $(this).closest('.modal');
                if ((xhr.getResponseHeader("content-type") || "").indexOf('html') > -1) {
                    $modal.find('.modal-dialog').html(data);
                } else if (formHandler.reloadOnSuccess === true) {
                    window.location.reload();
                } else {
                    $modal.modal('hide');
                }
            }

        $form.initAsAjaxForm(formHandler);
    },

    // initialize form as ajax
    /* formHandler is an optional object with these properties:
    *  {
    *  success: function(data) { ... }
    *  error: function(xhr, textStatus, errorThrown) { ... }
    *  validate: function() { ... } -- implementation should return null if no problems
    *  }
    */
    initAsAjaxForm: function (formHandler) {
        let $form = this;

        if (typeof formHandler === 'undefined') {
            formHandler = {};
        }

        // determine if the form contains an alert entry (otherwise, use
        // the error-dialog popup)
        let $alertField = $form.find('[role="alert"]');

        if (typeof formHandler.success !== 'function')
            formHandler.success = function (data, status, xhr) {
                if (handleAjaxResult($alertField, data, xhr))
                    window.location.reload();
            };
        else {
            let fncSuccess = formHandler.success;
            formHandler.success = function (data, status, xhr) {
                if (handleAjaxResult($alertField, data, xhr))
                    fncSuccess.call($form, data, status, xhr);
            };
        }

        if (typeof formHandler.error !== 'function')
            formHandler.error = function (xhr, textStatus, errorThrown) {
                let $p = $('<p>');
                $p.text(AjaxErrorToString(xhr, textStatus, errorThrown));
                showErrorDialog($alertField, $p);
            };

        $form.submit(function (e) {

            e.preventDefault();

            setTimeout(function () {

                if (typeof formHandler.validate === 'function') {
                    try {
                        var error = formHandler.validate.call($form);
                        if (error != null) {
                            formHandler.error(null, error, null);
                            return;
                        }
                    }
                    catch (e) {
                        formHandler.error(null, null, e);
                        return;
                    }
                }

                $submit = $form.find(':submit');
                var fd = new FormData($form[0]);   // serialize the form's elements

                if (typeof formHandler.prepareFetch === 'function') {
                    try {
                        formHandler.prepareFetch.call($form, fd);
                    }
                    catch (e) {
                        formHandler.error(null, null, e);
                        return;
                    }
                }

                $.ajax({
                    type: "POST",
                    url: $form.attr('action'),
                    data: fd,
                    contentType: false,
                    processData: false,
                    success: formHandler.success,
                    error: formHandler.error,
                    beforeSend: function () {
                        $submit.showWaitIcon();
                    },
                    complete: function () {
                        $submit.hideWaitIcon();
                    }
                });
            }, 0);
        });
    }
});

function handleAjaxResult(alertField, data, xhr) {
    if (!isDefined(xhr) || (xhr.getResponseHeader("content-type") || "").indexOf('json') > -1) {
        var result = createErrorList(data);

        if (result != null) {
            showErrorDialog(alertField, result);
            return false;
        }

        if (alertField != null && alertField.length > 0) {
            alertField.empty();
            alertField.addClass('d-none');
        }
    }
    return true;
}

function createErrorList(data) {
    if ( !isDefined(data) || data=="" ||data.r == 0 || !isDefined(data.r) )
        return null;

    var $ul = $('<ul>');

    if (data.m.constructor === Array) {
        data.m.forEach(function (searchresult) {
            var $li = $('<li>');
            $li.text(searchresult);
            $ul.append($li);
        });
    } else {
        var $li = $('<li>');
        $li.text(data.m);
        $ul.append($li);
    }

    return $ul;
}

function showErrorDialog(alertField, htmlMsg) {
    if (alertField != null && alertField.length > 0) {
        alertField.html(htmlMsg);
        alertField.removeClass('d-none');
    } else {
        var $d = $("#error-dialog");
        $d.find(".modal-body").html(htmlMsg);
        $d.modal('show');
    }
}

function AjaxErrorToString(xhr, textStatus, errorThrown) {
    return ((xhr != null) ? xhr.responseText : ((textStatus != null) ? textStatus : errorThrown.toString()));
}

function createErrorList(data) {
    if (!isDefined(data) || data == "" || data.r == 0 || !isDefined(data.r))
        return null;

    var $ul = $('<ul>');

    if (data.m.constructor === Array) {
        data.m.forEach(function (searchresult) {
            var $li = $('<li>');
            $li.text(searchresult);
            $ul.append($li);
        });
    } else {
        var $li = $('<li>');
        $li.text(data.m);
        $ul.append($li);
    }

    return $ul;
}

function isDefined(value) {
    return typeof value !== 'undefined' && value != null;
}

function datetimeFormatter(value) {
    return new Date(Date.parse(value)).toLocaleString();
}

function dateFormatter(value) {
    return new Date(Date.parse(value)).toLocaleDateString();
}

function dateRangeFormatter(value) {
    let s = value.split('|')
    let start = new Date(Date.parse(s[0]))
    let end = new Date(Date.parse(s[1]))

    if (start.getFullYear() == end.getFullYear() &&
        start.getMonth() == end.getMonth() &&
        start.getHours() == end.getHours()) {
        // same day
        return `${start.toLocaleDateString()} ${start.toLocaleTimeString()} ~ ${end.toLocaleTimeString}`
    } else {
        // date range
        return `${start.toLocaleDateString()} ~ ${end.toLocaleDateString() }`
    }
}

function timeFormatter(value) {
    return new Date(Date.parse(value)).toLocaleTimeString();
}
