// Print functionality for PDF documents
window.printPdf = (blob, printerName) => {
    // Create a hidden iframe for printing
    const iframe = document.createElement('iframe');
    iframe.style.position = 'fixed';
    iframe.style.right = '0';
    iframe.style.bottom = '0';
    iframe.style.width = '0';
    iframe.style.height = '0';
    iframe.style.border = '0';
    
    document.body.appendChild(iframe);
    
    const blobUrl = URL.createObjectURL(blob);
    iframe.src = blobUrl;
    
    iframe.onload = () => {
        try {
            iframe.contentWindow.focus();
            iframe.contentWindow.print();
            
            // Clean up after printing
            setTimeout(() => {
                document.body.removeChild(iframe);
                URL.revokeObjectURL(blobUrl);
            }, 1000);
        } catch (error) {
            console.error('Error printing PDF:', error);
            document.body.removeChild(iframe);
            URL.revokeObjectURL(blobUrl);
        }
    };
    
    iframe.onerror = () => {
        console.error('Error loading PDF for printing');
        document.body.removeChild(iframe);
        URL.revokeObjectURL(blobUrl);
    };
};

