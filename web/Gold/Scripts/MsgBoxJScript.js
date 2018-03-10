var styleToSelect;
function onOk() {
    $get('Paragraph1').className = styleToSelect;
}

// Add click handlers for buttons to show and hide modal popup on pageLoad
function pageLoad() {
    $addHandler($get("showModalPopupClientButton_Msg"), 'click', showModalPopupViaClient);
    $addHandler($get("hideModalPopupViaClientButton_Msg"), 'click', hideModalPopupViaClient);
}

function showModalPopupViaClient(ev) {
    ev.preventDefault();
    var modalPopupBehavior = $find('programmaticModalPopupBehavior_Msg');
    modalPopupBehavior.show();
}

function hideModalPopupViaClient(ev) {
    ev.preventDefault();
    var modalPopupBehavior = $find('programmaticModalPopupBehavior_Msg');
    modalPopupBehavior.hide();
}