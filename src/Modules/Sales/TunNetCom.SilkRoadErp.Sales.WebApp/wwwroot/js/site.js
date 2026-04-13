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

// Get available printers using browser APIs
window.getAvailablePrinters = async () => {
    try {
        // Check if Print API is available (Chrome/Edge with experimental features)
        // Note: This API is experimental and may not be available in all browsers
        if (navigator.print && typeof navigator.print.getPrinters === 'function') {
            try {
                const printers = await navigator.print.getPrinters();
                if (printers && printers.length > 0) {
                    return printers.map(p => ({
                        name: p.name || 'Unknown',
                        status: p.status || 'Ready',
                        isDefault: p.isDefault || false,
                        location: p.location || ''
                    }));
                }
            } catch (apiError) {
                console.debug('Print API getPrinters failed:', apiError);
            }
        }
        
        // For browsers that don't support printer enumeration,
        // we return an empty array and the C# code will provide a default option
        // The browser print dialog will show all printers when window.print() is called
        return [];
    } catch (error) {
        console.warn('Could not enumerate printers:', error);
        return [];
    }
};

// Enhanced print function that accepts base64 PDF and print options
window.printPdfFromBase64 = (base64String, copies = 1, duplex = false) => {
    try {
        // Convert base64 to blob
        const byteCharacters = atob(base64String);
        const byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        const blob = new Blob([byteArray], { type: 'application/pdf' });
        
        // Create a new window for printing
        const blobUrl = URL.createObjectURL(blob);
        const printWindow = window.open(blobUrl, '_blank');
        
        if (!printWindow) {
            // If popup was blocked, fall back to iframe method
            console.warn('Popup blocked, using iframe method');
            const iframe = document.createElement('iframe');
            iframe.style.position = 'fixed';
            iframe.style.right = '0';
            iframe.style.bottom = '0';
            iframe.style.width = '0';
            iframe.style.height = '0';
            iframe.style.border = '0';
            document.body.appendChild(iframe);
            
            iframe.src = blobUrl;
            
            iframe.onload = () => {
                try {
                    // Wait a bit for PDF to load
                    setTimeout(() => {
                        iframe.contentWindow.focus();
                        iframe.contentWindow.print();
                        
                        // Clean up after printing
                        setTimeout(() => {
                            if (document.body.contains(iframe)) {
                                document.body.removeChild(iframe);
                            }
                            URL.revokeObjectURL(blobUrl);
                        }, 1000);
                    }, 500);
                } catch (error) {
                    console.error('Error printing PDF:', error);
                    if (document.body.contains(iframe)) {
                        document.body.removeChild(iframe);
                    }
                    URL.revokeObjectURL(blobUrl);
                }
            };
            
            iframe.onerror = () => {
                console.error('Error loading PDF for printing');
                if (document.body.contains(iframe)) {
                    document.body.removeChild(iframe);
                }
                URL.revokeObjectURL(blobUrl);
            };
            
            return;
        }
        
        // Wait for the window to load, then print
        printWindow.onload = () => {
            try {
                // Small delay to ensure PDF is fully loaded
                setTimeout(() => {
                    printWindow.focus();
                    printWindow.print();
                    
                    // Clean up after printing
                    // Note: We don't close the window immediately to allow user to interact with print dialog
                    printWindow.addEventListener('afterprint', () => {
                        setTimeout(() => {
                            printWindow.close();
                            URL.revokeObjectURL(blobUrl);
                        }, 100);
                    });
                    
                    // Fallback cleanup if afterprint event doesn't fire
                    setTimeout(() => {
                        if (!printWindow.closed) {
                            printWindow.close();
                            URL.revokeObjectURL(blobUrl);
                        }
                    }, 30000); // 30 second timeout
                }, 500);
            } catch (error) {
                console.error('Error printing PDF:', error);
                printWindow.close();
                URL.revokeObjectURL(blobUrl);
            }
        };
        
        // Handle case where PDF loads before onload fires
        printWindow.addEventListener('load', () => {
            try {
                setTimeout(() => {
                    printWindow.focus();
                    printWindow.print();
                    
                    printWindow.addEventListener('afterprint', () => {
                        setTimeout(() => {
                            printWindow.close();
                            URL.revokeObjectURL(blobUrl);
                        }, 100);
                    });
                    
                    setTimeout(() => {
                        if (!printWindow.closed) {
                            printWindow.close();
                            URL.revokeObjectURL(blobUrl);
                        }
                    }, 30000);
                }, 500);
            } catch (error) {
                console.error('Error printing PDF:', error);
                printWindow.close();
                URL.revokeObjectURL(blobUrl);
            }
        });
        
    } catch (error) {
        console.error('Error in printPdfFromBase64:', error);
        throw error;
    }
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

// Camera capture functionality
window.captureImageFromCamera = function() {
    return new Promise(async (resolve, reject) => {
        try {
            // Check if getUserMedia is available
            // Support for older browsers (Firefox, Safari)
            const getUserMedia = navigator.mediaDevices?.getUserMedia || 
                                 navigator.getUserMedia || 
                                 navigator.webkitGetUserMedia || 
                                 navigator.mozGetUserMedia || 
                                 navigator.msGetUserMedia;
            
            if (!getUserMedia) {
                reject(new Error('Camera access is not supported in this browser. Please use Chrome, Edge, or Firefox with HTTPS.'));
                return;
            }
            
            // Wrap getUserMedia for compatibility
            const getUserMediaWrapper = (constraints) => {
                if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
                    return navigator.mediaDevices.getUserMedia(constraints);
                } else {
                    // Legacy API
                    return new Promise((resolve, reject) => {
                        const legacyGetUserMedia = navigator.getUserMedia || 
                                                   navigator.webkitGetUserMedia || 
                                                   navigator.mozGetUserMedia || 
                                                   navigator.msGetUserMedia;
                        legacyGetUserMedia.call(navigator, constraints, resolve, reject);
                    });
                }
            };

            // Request camera access
            const stream = await getUserMediaWrapper({ 
                video: { 
                    facingMode: 'environment' // Use back camera on mobile devices
                } 
            });

            // Create video element to show camera preview
            const video = document.createElement('video');
            video.srcObject = stream;
            video.autoplay = true;
            video.playsInline = true;
            video.style.width = '100%';
            video.style.maxWidth = '640px';
            video.style.height = 'auto';

            // Create container for preview and controls
            const container = document.createElement('div');
            container.style.position = 'fixed';
            container.style.top = '0';
            container.style.left = '0';
            container.style.width = '100%';
            container.style.height = '100%';
            container.style.backgroundColor = 'rgba(0, 0, 0, 0.9)';
            container.style.display = 'flex';
            container.style.flexDirection = 'column';
            container.style.alignItems = 'center';
            container.style.justifyContent = 'center';
            container.style.zIndex = '10000';

            // Create canvas for capturing
            const canvas = document.createElement('canvas');
            const ctx = canvas.getContext('2d');

            // Create capture button
            const captureButton = document.createElement('button');
            captureButton.textContent = 'Capturer';
            captureButton.style.padding = '15px 30px';
            captureButton.style.marginTop = '20px';
            captureButton.style.fontSize = '18px';
            captureButton.style.backgroundColor = '#4CAF50';
            captureButton.style.color = 'white';
            captureButton.style.border = 'none';
            captureButton.style.borderRadius = '5px';
            captureButton.style.cursor = 'pointer';

            // Create cancel button
            const cancelButton = document.createElement('button');
            cancelButton.textContent = 'Annuler';
            cancelButton.style.padding = '15px 30px';
            cancelButton.style.marginTop = '10px';
            cancelButton.style.fontSize = '18px';
            cancelButton.style.backgroundColor = '#f44336';
            cancelButton.style.color = 'white';
            cancelButton.style.border = 'none';
            cancelButton.style.borderRadius = '5px';
            cancelButton.style.cursor = 'pointer';

            // Create button container
            const buttonContainer = document.createElement('div');
            buttonContainer.style.display = 'flex';
            buttonContainer.style.gap = '10px';
            buttonContainer.appendChild(captureButton);
            buttonContainer.appendChild(cancelButton);

            container.appendChild(video);
            container.appendChild(buttonContainer);
            document.body.appendChild(container);

            const cleanup = () => {
                stream.getTracks().forEach(track => track.stop());
                if (document.body.contains(container)) {
                    document.body.removeChild(container);
                }
            };

            captureButton.onclick = () => {
                try {
                    // Set canvas dimensions to match video, but limit max size to reduce file size
                    const maxWidth = 1920;
                    const maxHeight = 1920;
                    let width = video.videoWidth;
                    let height = video.videoHeight;

                    // Scale down if too large
                    if (width > maxWidth || height > maxHeight) {
                        const ratio = Math.min(maxWidth / width, maxHeight / height);
                        width = width * ratio;
                        height = height * ratio;
                    }

                    canvas.width = width;
                    canvas.height = height;

                    // Draw video frame to canvas with scaling
                    ctx.drawImage(video, 0, 0, width, height);

                    // Convert to base64 with compression (0.7 quality for smaller file size)
                    const base64Image = canvas.toDataURL('image/jpeg', 0.7);
                    
                    // Extract base64 data (remove data:image/jpeg;base64, prefix)
                    const base64Data = base64Image.split(',')[1];
                    
                    cleanup();
                    resolve(base64Data);
                } catch (error) {
                    cleanup();
                    reject(error);
                }
            };

            cancelButton.onclick = () => {
                cleanup();
                reject(new Error('Camera capture cancelled'));
            };

            // Handle errors
            video.onerror = (error) => {
                cleanup();
                reject(error);
            };
        } catch (error) {
            reject(new Error(`Failed to access camera: ${error.message}`));
        }
    });
};

// Open document in new tab using blob URL (better for large files)
window.openDocumentInNewTab = function(dataUrl) {
    try {
        // Extract base64 data and content type
        const [header, base64Data] = dataUrl.split(',');
        const contentType = header.match(/data:([^;]+)/)?.[1] || 'application/octet-stream';
        
        // Convert base64 to blob
        const byteCharacters = atob(base64Data);
        const byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        const blob = new Blob([byteArray], { type: contentType });
        
        // Create blob URL
        const blobUrl = URL.createObjectURL(blob);
        
        // For images, create an HTML page with the image
        // For PDFs, open directly
        if (contentType.startsWith('image/')) {
            // Create a new window with HTML containing the image
            const newWindow = window.open('', '_blank');
            if (newWindow) {
                newWindow.document.write(`
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <title>Image Preview</title>
                        <style>
                            body {
                                margin: 0;
                                padding: 20px;
                                display: flex;
                                justify-content: center;
                                align-items: center;
                                min-height: 100vh;
                                background-color: #f5f5f5;
                            }
                            img {
                                max-width: 100%;
                                max-height: 100vh;
                                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                border-radius: 4px;
                            }
                        </style>
                    </head>
                    <body>
                        <img src="${blobUrl}" alt="Document preview" />
                    </body>
                    </html>
                `);
                newWindow.document.close();
                
                // Clean up blob URL after window loads
                newWindow.addEventListener('load', () => {
                    setTimeout(() => URL.revokeObjectURL(blobUrl), 2000);
                });
            } else {
                // If popup blocked, try opening data URL directly
                window.open(dataUrl, '_blank');
                setTimeout(() => URL.revokeObjectURL(blobUrl), 1000);
            }
        } else {
            // For PDFs and other documents, open blob URL directly
            const newWindow = window.open(blobUrl, '_blank');
            
            // Clean up blob URL after a delay (give browser time to load)
            if (newWindow) {
                newWindow.addEventListener('load', () => {
                    setTimeout(() => URL.revokeObjectURL(blobUrl), 2000);
                });
            } else {
                // If popup blocked, try direct download
                const link = document.createElement('a');
                link.href = blobUrl;
                link.target = '_blank';
                link.click();
                setTimeout(() => URL.revokeObjectURL(blobUrl), 1000);
            }
        }
    } catch (error) {
        console.error('Error opening document:', error);
        // Fallback: try opening data URL directly
        window.open(dataUrl, '_blank');
    }
};