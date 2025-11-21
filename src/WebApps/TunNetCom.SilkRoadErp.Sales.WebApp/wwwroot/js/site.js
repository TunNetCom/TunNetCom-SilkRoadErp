// wwwroot/js/site.js
window.downloadFile = (fileName, base64String, contentType) => {
    const byteCharacters = atob(base64String);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: contentType });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
};

window.setupOrderLinesGridKeyboard = (refId, dotNetRef) => {
    // Store the DotNetObjectReference
    window[refId] = dotNetRef;
    
    // Remove existing handler if it exists
    var existingHandler = window['orderLinesGridHandler_' + refId];
    if (existingHandler) {
        document.removeEventListener('keydown', existingHandler);
    }
    
    // Set up the keyboard handler
    var handler = function(e) {
        var target = e.target;
        if (target && (
            target.tagName === 'INPUT' || 
            target.tagName === 'TEXTAREA' ||
            target.isContentEditable ||
            target.closest('input') ||
            target.closest('textarea')
        )) {
            return;
        }

        if (e.key === 'Insert' || 
            (e.ctrlKey && (e.key === 'n' || e.key === 'N')) || 
            e.key === '+' ||
            (e.key === '=' && e.shiftKey)) {
            e.preventDefault();
            e.stopPropagation();
            var ref = window[refId];
            if (ref) {
                ref.invokeMethodAsync('HandleKeyboardInsert').catch(function(err) {
                    console.error('Error calling HandleKeyboardInsert:', err);
                });
            }
        }
    };

    document.addEventListener('keydown', handler);
    window['orderLinesGridHandler_' + refId] = handler;
};