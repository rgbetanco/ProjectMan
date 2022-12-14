// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


$(function () {

    var $d = $(document);

    // make modal toggles clickable
    $d.on('click', ".btn-modal", function (e) {
        let btn = $(this);
        let url = btn.data('url');
        let target = $(btn.data('target'));

        target.showRemoteModal(url, btn);
    })

    // make back buttons clickable
    $d.on("click", ".btn-back", function (e) {
        e.preventDefault();
        window.history.back();
    })

    // initialize tables
    for (let i of document.querySelectorAll('[data-cs-table]')) {
        let t = new CSTable(i, {
            nullDataMsg: '請選擇塞選條件...',
            emptyDataMsg: '無相關資料',
            downloadMsg: '下載中, 請稍後...',
            fetchOptions: { cache: "reload" }
        });
    }

    // handle form-group add button (that isn't a btn-modal)
    $d.on("click", "[data-form-group-target]:not(input[type=file]):not(.btn-modal)", function (e) {
        formGroupAdd( $(`[data-form-group-start="${$(this).data('formGroupTarget')}"]`)[0] );
        e.preventDefault();
    });

    // handle form-group file add
    $d.on("change", "input[data-form-group-target][type=file]", function (e) {
        let $this = $(this);
        let start = $(`[data-form-group-start="${$this.data('formGroupTarget')}"]`)[0];
        let ele = $this[0];
        const files = ele.files;

        if (files.length == 1) {
            // process single file
            let file = files.item(0);
            let reader = new FileReader();
            let $group = formGroupAdd(start);
            $group.find('[data-form-group-subname="file"]')[0].files = files;

            let $pic = $group.find('[data-form-group-tempname="pic"]');

            if ($pic.length) {
                reader.onload = e => {
                    $pic.attr("src", e.target.result);
                };
                reader.readAsDataURL(file);
            }

            let $name = $group.find('[data-form-group-tempname="name"]')
            $name.text(file.name)
        } else {
            // Loop through files and then split them into new inputs (not supported by safari iOS)
            for (let i = 0; i < files.length; i++) {
                let file = files.item(i);
                let reader = new FileReader();
                let $group = formGroupAdd(start);

                // new DataTransfer
                let dt = new DataTransfer();

                // add current file to it
                dt.items.add(file);

                // and set it to the hidden input
                $group.find('[data-form-group-subname="file"]')[0].files = dt.files;

                let $pic = $group.find('[data-form-group-tempname="pic"]');

                if ($pic.length) {
                    reader.onload = e => {
                        $pic.attr("src", e.target.result);
                    };
                    reader.readAsDataURL(file);
                }

                let $name = $group.find('[data-form-group-tempname="name"]')
                $name.text(file.name)
            }
        }
        ele.files = null;
    });

    // initialize hidden utc date <-> datetime-local input controls
    let $date_input = $d.find('input[type="datetime-local"][data-utc-target]');
    updateDateTimeControl( $date_input );

    $d.on('change', 'input[type="datetime-local"][data-utc-target]', function () {
        let $e = $(this);
        let $field = $($e.attr('data-utc-target'));

        const tempDate = new Date(`${$e.val()}Z`);
        const date = new Date(tempDate.getTime() + tempDate.getTimezoneOffset() * 60000);

        try {
            $field.val(date.toISOString());
        } catch (e) {

        }
    });

    // handle form group removal button click
    $d.on('click', '[data-form-group] .btn-delete', function () {
        formGroupRemove(this)
    });

    // localize data-datetime items
    $('[data-datetime]').each(function () {
        let $me = $(this);
        $me.text(datetimeFormatter($me.data('datetime')));
    });

    $('[data-date]').each(function () {
        let $me = $(this);
        $me.text(dateFormatter($me.data('date')));
    });

    $('[data-date-range]').each(function () {
        let $me = $(this);
        $me.text(dateRangeFormatter($me.data('dateRange')));
    });

    // handle toggle footer
    let $footerShow = $('#footer-show');
    let $footerHide = $('#footer-hide');
    let $footer = $('footer');

    function footerShow(show) {
        if (show == 1) {
            $footerShow.addClass('d-none');
            $footerHide.removeClass('d-none');
            $footer.removeClass('d-none');
        } else {
            $footerShow.removeClass('d-none');
            $footerHide.addClass('d-none');
            $footer.addClass('d-none');
        }
        localStorage.setItem("footer-state", show);
    }

    $footerShow.click(() => {
        footerShow(1);
    });

    $footerHide.click(() => {
        footerShow(0);
    });

    let footerState = localStorage.getItem("footer-state");
    footerShow((footerState != null) ? footerState : 1);
});

var _onAutoReloadFnc = function (event) {
    var historyTraversal = event.persisted ||
        (typeof window.performance != "undefined" &&
            window.performance.navigation.type === 2);
    if (historyTraversal) {
        if (typeof reloadNow === "function") {
            reloadNow()
        } else {
            window.location.reload()
        }
    }
}

// call this function on page start to force the page to reload whenever the user
// backs into it from another page
function enableAutoReload() {
    window.removeEventListener("pageshow", _onAutoReloadFnc)
    window.addEventListener("pageshow", _onAutoReloadFnc, false)
}

// update datetime input field using hidden utc date input field
function updateDateTimeControl( $date_input ) {
    $date_input.each(function (idx, ele) {
        let $e = $(ele);
        let d = new Date($($e.attr('data-utc-target')).val());
        try {
            let date = (new Date(d.getTime() - d.getTimezoneOffset() * 60000).toISOString()).slice(0, -1).replace(/\.000$/, "");
            $e.val(date);
        } catch (e) {

        }
    });
}

// sidebar functionality

function sidebarCollapseBtnClicked(e) {
    var target = $(e).data("target");
    $(target).toggleClass('sidebar-collapsed');
    localStorage.setItem("sidebar-state-" + target, $(target).hasClass('sidebar-collapsed') ? 1 : 0);
}

var _noop = function (e) { e.preventDefault(); }

// Collapse/Expand icon
try {
    var $sidebarCollapseBtn = $('[data-toggle=sidebar-collapse]');

    // handle click on sidebar collapse button
    // - all elements with a class 'collapsible' will be toggled by this button
    // - the collapse button needs to have a 'data-target' to mark the parent sidebar object
    $sidebarCollapseBtn.click(function () {
        sidebarCollapseBtnClicked(this);
        $(window).trigger('resize');
    });

    $sidebarCollapseBtn.each(function (index) {
        var target = $(this).data("target");

        var state = localStorage.getItem("sidebar-state-" + target);

        if (state == 1)
            sidebarCollapseBtnClicked(this);
    });
}
catch(e) {
}


// Enum Flag Multiselect Checkbox/Dropdown helper
jQuery.fn.extend({

    initAsEnumFlagMultiselect: function (options) {
        var ctrl = this;

        if (options == null)
            options = {};

        var onChange = function (e, checked) {
            if (!checked)
                return;

            var selectedValues = ctrl.find('option:selected').map(function (a, item) { return parseInt(item.value); });
            var curValue = e.val();
            var combined = curValue;

            // deselect redundant subsets of new selection
            $.each(selectedValues, function (i, n) {
                if (n == curValue)
                    return;

                if ((curValue & n) == n)    // deselect subset
                    ctrl.multiselect('deselect', n);
                else if ((n & curValue) == curValue)
                    ctrl.multiselect('deselect', n);
                else
                    combined |= n;
            });

            // see if new selection results in a superset being matched
            if (combined != curValue) {
                var unselectedValues = ctrl.find('option:not(:selected)').map(function (a, item) { return parseInt(item.value); });
                $.each(unselectedValues, function (i, n) {
                    if ((combined & n) == n)
                        ctrl.multiselect('select', n, true);
                });
            }
        };

        // if user already defined an onchange in the options, combine our onChange with it
        if (typeof options.onChange === 'function') {
            var prevOnChange = options.onChange;
            var newOnChange = function (e, checked) {
                prevOnChange(e, checked);
                onChange(e, checked);
            }

            onChange = newOnChange;
        }

        options.onChange = onChange;

        ctrl.initAsEnumMultiselect(options);
    },
    getCombinedEnumFlag: function () {
        var $f = this;
        var combinedValues = 0;
        var selectedValues = $f.find('option:selected').map(function (a, item) { return parseInt(item.value); });
        $.each(selectedValues, function (i, n) {
            combinedValues |= n;
        });

        return combinedValues;
    },


    initAsEnumMultiselect: function (options) {
        var $c = this;

        if (options == null)
            options = {};

        // this is meant to keep the dropbox in a separate line from the label
        options.buttonContainer = '<div class="btn-group w-100" />';

        $c.multiselect(options);
    },

    // loads selection data from the server via POST
    loadSelect: function (url, data, options) {

        if ( !isDefined(options) )
            options = {};

        let $this = this;
        let waiting = $('<option></option').val('').text("請稍後...");
        $this.empty().append(waiting).show();

        $.ajax({
            url: url,
            data: data,
            cache: false,
            type: "POST",
            success: function (data) {
                var markups = $(document.createDocumentFragment());

                if (options.blankOption) {
                    markups.append($('<option></option').val('').text(options.blankOption) );
                }

                for (var x = 0; x < data.length; x++) {
                    markups.append($('<option></option').val(data[x].value).text(data[x].text));
                }
                $this.empty().append(markups);
            },
            error: function (xhr, status, error) {
                let waiting = $('<option></option').val('').text(`錯誤 ${xhr.responseText}`);
                $this.empty().append(waiting).show();
            }
        });
    }

});


// Print Handling
$(function () {
    var beforePrint = function () {
        $b = $('.body');
        $b.find('.watermark').remove();

        var h = $b.height();
        var $watermarks = $('<div class="watermark"></div>');

        var $t = $('<h5>使用者: ' + _user +' ' +
            'IP來源: ' + _ip +'<br/>' +
            '列印日期與時間: ' + (new Date()).toviseString() + '</h5>');
        for (let i = 50; i < h - 99; i += 250) {
            $t2 = $t.clone();
            $t2.css({
                top: '' + i + 'px',
            });

            $watermarks.append($t2);
        }

        $('.body').append($watermarks);
    };
    var afterPrint = function () {
        $b.find('.watermark').remove();
    };

    if (window.matchMedia) {
        var mediaQueryList = window.matchMedia('print');
        mediaQueryList.addListener(function (mql) {
            if (mql.matches) {
                beforePrint();
            } else {
                afterPrint();
            }
        });
    }

    window.onbeforeprint = beforePrint;
    window.onafterprint = afterPrint;
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

// AJAX modal dialog functionality
jQuery.fn.extend({

    // downloads and displays a modal dialog from a given url, using
    // current element to show the modal dialog.  Results are returned to the
    // optional resultTarget element
    showRemoteModal: function (url, $resultTarget) {
        let $target = this;

        $.get(url).done( data => {

            let $formTarget = null, $formGroupTarget = null, $formParent = null
            if (typeof $resultTarget.data('formGroupTarget') !== 'undefined') {
                let formGroupTarget = $resultTarget.data('formGroupTarget');
                $formTarget = $resultTarget.closest(`[data-form-group="${formGroupTarget}"]`)
                if ($formTarget.length > 0) {
                    if (typeof $formTarget.data('formGroupStart') !== 'undefined') {
                        [$formGroupTarget, $formParent] = _formGroupPrepareAdd($formTarget[0]);
                        $formTarget = $formGroupTarget 
                    }
                } else {
                    [$formGroupTarget, $formParent] = _formGroupPrepareAdd($(`[data-form-group-start="${formGroupTarget}"]`)[0]);
                    $formTarget = $formGroupTarget 
                }

            } else if (typeof $resultTarget.data('formTarget') !== 'undefined') {
                $formTarget = $($resultTarget.data('formTarget'))
            }

            $target.on('hidden.bs.modal', function (e) {
                $target.off()
                $target.find('*').off()  // remove handlers attached to children of the modal dialog when done

                if ($target.data('csModalSuccess')) {
                    // if this button is an add button, use it to update the form field
                    let data = $target.data('csModalData')

                    // fire a modal-success event; if preventDefault isn't called,
                    // then we'll automatically try to set form field values based on
                    // the data returned by the modal dialog
                    let event = $.Event('modal-success')
                    $resultTarget.trigger(event, [ $formTarget, data] )

                    // if no handler is preventing default, then we'll apply data 
                    if (!event.isDefaultPrevented() && $formTarget != null) {
                        formGroupApplyData($formTarget, data)
                    }

                    // if there is a form group to show, finalize it now
                    if ($formGroupTarget != null)
                        _formGroupFinalizeAdd($formGroupTarget, $formParent)
                }
            })

            // clear results (in case this object was used before)
            $target.data('csModalSuccess', false)
            $target.data('csModalData', null)

            // set source data
            $target.data('csSource', $formTarget)

            // display modal
            $target.find(".modal-dialog").html(data)
            $target.modal("show")
        })
    },

    // initializes a modal dialog to perform ajax operations with server
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

    dismissModal: function (is_success, data) {
        let $modal = this.closest('.modal');
        $modal.data('csModalSuccess', is_success);
        $modal.data('csModalData', data);
        $modal.modal('hide');
    },

    initDismissModalButton: function (is_success, fncOnClick) {
        this.on('click', evt => {
            let $target = $(evt.target);
            let data = fncOnClick.call($target.closest('.modal'));
            $target.dismissModal( is_success, data )
        });
    }
})


// AJAX and toggleable Form functionality

jQuery.fn.extend({

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

        if (typeof formHandler === 'undefined' ) {
            formHandler = {};
        }

        // determine if the form contains an alert entry (otherwise, use
        // the error-dialog popup)
        let $alertField = $form.find('[role="alert"]');

        if (typeof formHandler.success !== 'function')
            formHandler.success = function (data) {
                if (handleAjaxResult($alertField, data))
                    window.location.reload();
            };
        else {
            let fncSuccess = formHandler.success;
            formHandler.success = function (data) {
                if (handleAjaxResult($alertField, data))
                    fncSuccess.call($form, data);
            };
        }

        if (typeof formHandler.error !== 'function')
            formHandler.error = function (xhr, textStatus, errorThrown) {
                let $p = $('<p>');
                $p.text(AjaxErrorToString(xhr, textStatus, errorThrown));
                showErrorDialog($alertField, $p);
            };

        // see if we have a form table to handle
        let $table = $form.find('table[data-form-table-group]');
        if ($table.length != 0) {
            $table.on('focus', 'td[contenteditable="true"]', function () {
                let cell = this;
                // select all text in contenteditable
                // see http://stackoverflow.com/a/6150060/145346
                let range, selection;
                if (document.body.createTextRange) {
                    range = document.body.createTextRange();
                    range.moveToElementText(cell);
                    range.select();
                } else if (window.getSelection) {
                    selection = window.getSelection();
                    range = document.createRange();
                    range.selectNodeContents(cell);
                    selection.removeAllRanges();
                    selection.addRange(range);
                }
            });
            $table.on('click', 'td .btn-delete', function (e) {
                let $btnDelete = $(this);
                let $row = $btnDelete.parents('tr');

                // if element has a key set, hide it;
                // otherwise, remove it it.
                if ($row.attr('data-cs-key')) {
                    $row.addClass('d-none');
                } else {
                    $row.remove();
                }

                e.preventDefault();
            });
            $table.on('input', 'td[contenteditable="true"]', function () {
                var $this = $(this);
                var $btnDelete = $this.parents('tr').find('td button.btn-delete');
                if ($btnDelete.hasClass('d-none')) {
                    $btnDelete.removeClass('d-none');
                    $table.addEditTableRow();
                }
            });

            $table.addEditTableRow();
        }

        let $submit = $form.find(':submit');
        var pressed_button_value = null;

        $submit.click(function () {
            pressed_button_value = $(this).val();
        })

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

                var fd = new FormData($form[0]);   // serialize the form's elements

                // if submit button has a value, set form's submit to that value
                if (pressed_button_value != null) {
                    fd.set('submit', pressed_button_value)
                }

                if (typeof formHandler.prepareFetch === 'function') {
                    try {
                        formHandler.prepareFetch.call($form, fd);
                    }
                    catch (e) {
                        formHandler.error(null, null, e);
                        return;
                    }
                }

                function getValue($col) {

                    let newVal;

                    // see if we're using an input control within the cell
                    if (!$col[0].isContentEditable) {
                        let $input = $col.find(':input:first');
                        if ($input.length) {
                            if ($input.is('[type="checkbox"]'))
                                newVal = $input.is(":checked") ? 'true' : 'false';
                            else {
                                newVal = $input.val();
                                if ($.isArray(newVal))
                                    newVal = newVal.join(',');
                            }

                            // remove name associated with the input
                            if ($input.attr('name')) {
                                fd.delete($input.attr('name'));
                            }
                        }
                    } else {
                        newVal = $col.text();
                    }

                    return newVal;
                }

                // handle any table forms found within the form
                var $tables = $form.find('table[data-form-table-group]');
                $tables.each(function (idx) {
                    let mappingField = [];
                    let mappingTarget = [];
                    let mappingTargetName = [null, 'Field', 'Row'];

                    let $table = $(this);

                    let prefix = $table.attr('data-form-table-group') ? $table.data('form-table-group') : "";
                    if (prefix.length != 0)
                        prefix = prefix + ".";

                    // get all the field names
                    $header = $table.find('thead tr');
                    $header.children().each(function (idx) {
                        let $itm = $(this);

                        if ($itm.attr('data-cs-field')) {
                            mappingTarget.push(1);
                            mappingField.push($itm.data('csField'));
                        } else if ($itm.attr('data-cs-row')) {
                            mappingTarget.push(2);
                            mappingField.push($itm.data('csRow'));
                        } else {
                            mappingTarget.push(null);
                            mappingField.push(null);
                        }
                    });

                    let add = 0, del = 0, mod = 0;
                    $table.find('tbody tr').each(function (idx) {
                        let $itm = $(this);

                        // ignore rows that don't have data yet
                        if ($itm.find('td button.btn-delete').hasClass('d-none'))
                            return;

                        if ($itm.attr('data-cs-key')) {
                            // mod or del
                            if ($itm.hasClass('d-none')) {
                                // deleting
                                fd.append(`${prefix}Del[${del++}]`, $itm.data('csKey'));
                            } else {
                                // modifying
                                let modCol = [0, 0, 0];

                                $itm.children().each(function (idx) {
                                    let $col = $(this);
                                    if (mappingTarget[idx] != null) {

                                        let newVal = getValue($col);

                                        if (newVal != $col.data('csOrigValue')) {
                                            let targetCol = mappingTarget[idx];
                                            let targetColName = mappingTargetName[mappingTarget[idx]];
                                            fd.append(`${prefix}Mod[${mod}].${targetColName}[${modCol[targetCol]}].Key`, mappingField[idx]);
                                            fd.append(`${prefix}Mod[${mod}].${targetColName}[${modCol[targetCol]++}].Value`, newVal);
                                        }
                                    }
                                });

                                if ((modCol[1] + modCol[2]) != 0) {
                                    fd.append(`${prefix}Mod[${mod++}].ID`, $itm.data('csKey'));
                                }

                            }
                        } else {
                            // new
                            let modCol = [0, 0, 0];

                            $itm.children().each(function (idx) {
                                let $col = $(this);
                                if (mappingTarget[idx] != null) {

                                    let newVal = getValue($col);

                                    if (newVal != '' && mappingTarget[idx] != null) {
                                        let targetCol = mappingTarget[idx];
                                        let targetColName = mappingTargetName[mappingTarget[idx]];
                                        fd.append(`${prefix}Add[${add}].${targetColName}[${modCol[targetCol]}].Key`, mappingField[idx]);
                                        fd.append(`${prefix}Add[${add}].${targetColName}[${modCol[targetCol]++}].Value`, newVal);
                                    }
                                }
                            });

                            if ((modCol[1] + modCol[2]) != 0) {
                                add++;
                            }
                        }
                    });
                });

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
    },

    // initialize form as a toggleable form
    // a button with ID #[form_id]-enable-button will be used to enable the form
    initAsToggleForm: function (enableNow, formHandler) {

        var $f = this;
        $f.initAsAjaxForm(formHandler);

        $f.on("reset", function () {
            var $cb = $f.find(".form-check").find(":checkbox,:radio");
            $cb.each(function () {
                $(this).prop('checked', $(this).prop('defaultChecked'));
            });
            $f.enableForm(false);

            setTimeout( function() {
                // initialize hidden utc date <-> datetime-local input controls
                let $date_input = $f.find('input[type="datetime-local"][data-utc-target]');
                updateDateTimeControl( $date_input );
            } )
        });

        $f.initFormGroupResetButton();

        // if form doesn't have a data-enable-button attribute set,
        // then default to #[form_id]-enable-button
        if (!$f.data('enable-button')) {
            $f.data('enable-button', "#" + $f.attr('id') + "-enable-button");
        }

        $($f.data('enable-button'))
            .click(function () { $f.enableForm(true); });

        $f.enableForm(enableNow);
    },

    enableForm: function (enable) {
        let f = this;
        let $t = f.find('table[data-form-table-group]');

        $(f.data('enable-button'))
            .prop("disabled", enable);

        f.find(".btn").prop("disabled", !enable);
        f.find(".form-select").prop("disabled", !enable);

        let $fc = f.find(".form-check");
        if (enable) {
            f.find(".btn").show();
            f.addClass('form-editmode');
            f.removeClass('form-viewmode');

            f.find(".form-control-plaintext")
                .removeClass("form-control-plaintext")
                .addClass("form-control")
                .prop("readonly", false);

            $fc.show();
            $fc.find(":checkbox,:radio").prop("disabled", false);
            $fc.removeClass("form-check-inline");

            // enable editable content
            $t.find('[data-edittable-orig]').prop('contenteditable', true);
        } else {
            f.find(".btn").hide();
            f.addClass('form-viewmode');
            f.removeClass('form-editmode');

            f.find(".form-control")
                .addClass("form-control-plaintext")
                .removeClass("form-control")
                .prop("readonly", true);

            f.find(".form-check:has(:checkbox:not(:checked),:radio:not(:checked)):not(.form-check-alwaysshow)").hide();
            $fc.find(":checkbox,:radio").prop("disabled", true);
            $fc.addClass("form-check-inline");

            // disable editable content
            $t.find('[data-edittable-orig]').prop('contenteditable', false);
        }

        f.trigger('form-enable', { enable : enable } );
    },

    initFormGroupResetButton: function () {
        let f = this;
        f.find('table[data-form-table-group]').find('[contenteditable]').each(function () {
            let $this = $(this);
            $this.attr('data-edittable-orig', $this.text());
        });

        f.on("reset", function () {
                // find all elements with data-form-group in the form
                var group = f.find("[data-form-group]");

                group.each(function (idx) {
                    var g = $(this);
                    var groupName = g.data("form-group");

                    // check if this group came from the server
                    var index = g.find('[name="' + groupName + '.Index"]');
                    if (index != null && index.length>0) {
                        var arrayName = groupName + '[' + index.val() + ']';
                        var id = g.find('[name="' + arrayName + '.ID"]');
                        if (id != null && id.length>0 ) {
                            g.find('[name="' + arrayName + '.Deleted"]').remove();
                            g.show();
                            return;
                        }
                    }

                    // if found inside data-form-group-start, don't remove!
                    var formGroupTemplate = g.closest('[data-form-group-start="'+ groupName +'"]');
                    if (formGroupTemplate != null && formGroupTemplate.length>0)
                        return;

                    g.remove();
                });

                // handle table reset
                f.find('table[data-form-table-group]').find('[data-edittable-orig]').each(function () {
                    let $this = $(this);
                    $this.text( $this.data('edittable-orig') );
                });
            });
    },

    addEditTableRow: function () {
        var $table = this;

        // see if we have the hidden element required to add a new row
        if (!$table.has('[data-form-table-row]'))
            return;

        var $group = $table.find('[data-form-table-row]').clone(true);
        $group.removeClass('d-none');
        $group.removeAttr('data-form-table-row');
        $group.appendTo($table.find('tbody'));
    }

})

// dynamic form field creation/deletion (i.e. named form groups)

// handles addition of a new form row
// 1. the function will search for a parent (A) with the attribute data-form-group-start
// 2. from there, find a child element (B) with the attribute data-form-group  --> this will be cloned and added right before (A)
//    data-form-group => name of this form group
// 3. find all children of (B) that contains the attribute data-form-group-subname, and add name=[data-form-group][index].[data-form-group-subname]
// 4. add a hidden field with name=[data-form-group].Index = index
// 5. add the new group to the DOM, and trigger 'form-group-add' event for the new group
// 6. if a handler for the trigger is preventing default, remove the group from DOM and return
// 7. trigger 'form-group-added' event on the new [data-form-group] element
function formGroupAdd(element) {
    let [$group, $parent] = _formGroupPrepareAdd(element);

    if (_formGroupFinalizeAdd($group, $parent))
        return $group;

    return null;
}

function _formGroupPrepareAdd(element) {
    var btn = $(element);
    var parent = btn.closest("[data-form-group-start]");
    var $group = (parent.is("[data-form-group]") ? parent : parent.find("[data-form-group]")).clone(true);
    $group.removeClass("d-none");
    $group.removeAttr("data-form-group-start");
    $group.removeData("form-group-start");

    // generate an index for the fields
    index = (typeof btn.data('form-group-start-index') !== 'undefined') ? (parseInt(btn.data("form-group-start-index")) + 1) : 1;
    btn.data("form-group-start-index", index);
    index = "new" + index;
    var groupName = $group.data("form-group");

    // create indexed names for the new fields
    var childFields = $group.find("[data-form-group-subname]");
    childFields.each(function (idx, e) {
        let $e = $(e)
        let newName = `${groupName}[${index}].${$e.data("form-group-subname")}`

        if ($e.is('[data-utc-target]')) {
            // handle local date time input
            let $actualE = $group.find($e.attr('data-utc-target'));
            let newID = newName.replace(/[\[\].]/g, '_');
            $actualE.attr("id", newID);
            $e.attr('data-utc-target', '#' + newID);
            $e = $actualE;
        }
        $e.attr('name', newName );
    });

    // record index with the new fields
    $group.prepend(`<input type="hidden" name="${groupName}.Index" value="${index}" />`);

    return [$group, parent]
}

function _formGroupFinalizeAdd($group, $parent) {
    let event = $.Event('form-group-add');
    var childFields = $group.find("input[data-form-group-subname]");

    // add to form
    $group.insertBefore($parent);

    $group.trigger(event);

    if (event.isDefaultPrevented()) {
        $parent.remove($group)
        return false;    // if default is prevented, then form group doesn't get added
    }

    childFields.first().focus();

    $group.trigger('form-group-added');

    return true
}

// handles removal of an existing row
// 1. the function will search for a parent (A) with the attribute data-form-group
//    data-form-group => name of this form group
// 2. trigger 'form-group-remove' event on this [data-form-group] element
// 3. if trigger is prevented by a handler, stop without removing the group
// 4. from there, look for (I) = [data-form-group].Index, and use that to find [data-form-group][(I)].ID
// 5. if found, that means this group was created by the server:
//    a. add [data-form-group][(I)].Deleted = true so that server knows it needs to be deleted
//    b. hide this form group
// 6. if not found, simply delete this group and return
// 7. trigger 'form-group-removed' event right before it is removed
function formGroupRemove(element) {
    let $ele = $(element);
    let event = $.Event('form-group-remove');
    var $group = formGroupGetFromChild($ele);
    $group.trigger(event);

    if (event.isDefaultPrevented()) {
        return false;    // if default is prevented, then form group doesn't get removed
    }

    $parent = $group.parent();

    // check if this group came from the server
    var prefix = formGroupGetPrefix($group);
    if (prefix != null) {
        var id = $group.find(`[name="${prefix}.ID"]`);
        if (id != null && id.length > 0) {
            $group.prepend(`<input type="hidden" name="${prefix}.Deleted" value="true" />`);
            $group.hide();
            return;
        }
    }

    $group.trigger('form-group-removed');
    $group.remove();
}

function formGroupGetFromChild($child) {
    return $child.closest('[data-form-group]');
}

function formGroupGetChildByName($formGroup, name, elementType = 'input') {
    let prefix = formGroupGetPrefix($formGroup);
    return $formGroup.find(`${elementType}[name='${prefix}.${name}']`);
}

function formGroupGetPrefix($formGroup) {
    let groupName = $formGroup.data("form-group");
    let index = $formGroup.find(`[name="${groupName}.Index"]`);
    return (index != null && index.length > 0) ? `${groupName}[${index.val()}]` : null;
}

// uses a list of name to data mapping to set the fields in the given form
// if name starts with *, then the field is only modified if it is currently empty
function formGroupApplyData($formGroup, data) {
    let prefix = formGroupGetPrefix($formGroup);
    prefix = (prefix != null) ? `${prefix}.` : ""

    $.each(data, function (name, value) {
        let optional = name.startsWith('*')
        let subname = (optional) ? name.substring(1) : name
        let $e = $formGroup.find(`[name='${prefix}${subname}']`)

        if ($e.length == 0 ) {
            // see if we can find using data-form-group-subname
            $e = $formGroup.find(`[data-form-group-subname='${subname}']`);
        }

        $e.each(function (idx) {
            let $this = $(this);
            if ($this.is(":checkbox") || $this.is(":radio")) {
                // checkboxes and radio buttons will always be set even if the field is optional
                if (value) {
                    $this.prop('checked', value);
                } else {
                    $this.removeProp('checked');
                }
            } else {
                if ($this.is('[data-utc-target]')) {
                // targetting local date display -- change hidden input instead
                    let $target = $e.find($this.attr('data-utc-target'));

                    if (!optional || $target.val() == '') {
                        $target.val(value);
                        updateDateTimeControl($this);
                    }

                } else {
                    if (!optional || $this.val() == '') {
                        if ($this.is("input"))
                            $this.val(value);
                        else
                            $this.text(value);

                        // update local date display if needed
                        let $dateDisplay = $e.find(`[data-utc-target="#${$this.attr('id')}"]`);
                        updateDateTimeControl($dateDisplay)
                    }
                }

            }
        })
    });
}

// helper functions
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
        return `${start.toLocaleDateString()} ~ ${end.toLocaleDateString()}`
    }
}

function timeFormatter(value) {
    return new Date(Date.parse(value)).toLocaleTimeString();
}

// error message handling

function AjaxErrorToString(xhr, textStatus, errorThrown) {
    return "錯誤: " + ((xhr != null) ? xhr.responseText : ((textStatus != null) ? textStatus : errorThrown.toString()));
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

function handleAjaxResult( alertField, data, xhr) {
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