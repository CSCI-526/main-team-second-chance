// This is a Google Apps Script that imports data from a Firebase Realtime Database into a Google Sheet.
// The script prompts the user to enter a game version number, fetches the data from Firebase, and populates the sheet with the data.
// The script also creates charts for each metric automatically.
// This version includes support for Vector2 values (objects with x/y properties) and handles primitive data types.

function formatKey(key) {
    return key.split("_").map(word => word[0].toUpperCase() + word.slice(1)).join(" ");
}

function importFirebaseData() {
    const ui = SpreadsheetApp.getUi();
    const spreadsheet = SpreadsheetApp.getActiveSpreadsheet();
    const dataSheet = spreadsheet.getActiveSheet(); // Use a specific variable for the main data sheet

    // --- Configuration ---
    const projectId = "team-second-chance";
    // IMPORTANT: Consider storing API Key more securely if needed (e.g., Properties Service)
    // For RTDB, security is often primarily handled by Firebase Rules.
    const apiKey = "AIzaSyAoXTy6p6rtSSHtduwgQ86zpAdlCNtq08w";
    const defaultDataKeyForPrimitives = "value"; // Key used when entry.data is not an object
    // ---------------------

    const versionResponse = ui.prompt(
        'Select Game Version',
        'Enter the game version number (e.g., 1, 2, 3):',
        ui.ButtonSet.OK_CANCEL
    );

    if (versionResponse.getSelectedButton() !== ui.Button.OK) {
        Logger.log('Game version selection was canceled.');
        return;
    }

    const gameVersion = versionResponse.getResponseText().trim();
    if (!gameVersion || isNaN(gameVersion)) {
        ui.alert('Error', 'Please enter a valid game version number.', ui.ButtonSet.OK);
        return;
    }

    const baseUrl = `https://${projectId}-default-rtdb.firebaseio.com/${gameVersion}.json`;
    const url = baseUrl + "?auth=" + apiKey;

    try {
        const response = UrlFetchApp.fetch(url, { muteHttpExceptions: true }); // Mute exceptions to check response code
        const responseCode = response.getResponseCode();
        const responseText = response.getContentText();

        if (responseCode !== 200) {
            ui.alert('Firebase Error', `Failed to fetch data. Response code: ${responseCode}. Response: ${responseText}`, ui.ButtonSet.OK);
            Logger.log(`Firebase Error: Code ${responseCode}, Response: ${responseText}`);
            return;
        }

        const data = JSON.parse(responseText);

        if (!data || Object.keys(data).length === 0) {
            ui.alert('No Data', `No data found for game version ${gameVersion}.`, ui.ButtonSet.OK);
            return;
        }

        Logger.log(`Fetched data for version ${gameVersion}. Processing...`);

        const metricEntries = [];
        const allTopLevelKeys = new Set();
        const uniqueMetricIds = new Set();
        const vector2Fields = new Set(); // Track fields that contain Vector2 data

        // --- Data Processing Loop ---
        for (const deviceId in data) {
            if (!data[deviceId]) continue; // Skip null/empty device entries
            for (const sessionId in data[deviceId]) {
                if (!data[deviceId][sessionId]) continue; // Skip null/empty session entries
                for (const metricId in data[deviceId][sessionId]) {
                    if (!data[deviceId][sessionId][metricId]) continue; // Skip null/empty metric entries

                    uniqueMetricIds.add(metricId); // Add to our set of unique metrics

                    for (const entryId in data[deviceId][sessionId][metricId]) {
                        const entry = data[deviceId][sessionId][metricId][entryId];
                        if (!entry || typeof entry.timestamp === 'undefined') continue; // Skip invalid entries

                        const timestamp = entry.timestamp;
                        let processedData = {};

                        // Standardize data: Ensure it's always an object
                        if (typeof entry.data === 'object' && entry.data !== null) {
                            processedData[metricId] = entry.data.value;
                        } else {
                            // Wrap primitive data (number, string, boolean, null) in a standard object
                            processedData[metricId] = entry.data;
                        }

                        // Discover keys from the processed data object
                        Object.keys(processedData).forEach(key => {
                            allTopLevelKeys.add(key);

                            // Check if this key represents a Vector2 object
                            const value = processedData[key];
                            if (
                                typeof value === 'object' &&
                                value !== null &&
                                'x' in value &&
                                'y' in value &&
                                !isNaN(Number(value.x)) && // Check if x is number-like
                                !isNaN(Number(value.y))    // Check if y is number-like
                            ) {
                                vector2Fields.add(key);
                            }
                        });

                        metricEntries.push({
                            gameVersion: gameVersion,
                            timestamp: new Date(timestamp), // Keep as Date object for sorting
                            deviceId: deviceId,
                            sessionId: sessionId,
                            metricId: metricId,
                            rawData: processedData // Store the processed data object
                        });
                    }
                }
            }
        }
        // --- End Data Processing Loop ---

        if (metricEntries.length === 0) {
            ui.alert('No Metric Entries', `No valid metric entries found in the data for game version ${gameVersion}.`, ui.ButtonSet.OK);
            return;
        }

        // Convert the set of keys to an array and sort alphabetically
        const topLevelKeys = Array.from(allTopLevelKeys).sort();
        const vector2FieldsArray = Array.from(vector2Fields);
        Logger.log(`Found ${metricEntries.length} entries, ${uniqueMetricIds.size} unique metrics.`);
        Logger.log(`Discovered data keys: ${topLevelKeys.join(', ')}`);
        Logger.log(`Identified Vector2 keys: ${vector2FieldsArray.join(', ')}`);


        // --- Header Creation ---
        const baseHeaders = ["Game Version", "Timestamp", "Device ID", "Session ID", "Metric ID", "Raw JSON Data"];
        const dataHeaders = [];
        topLevelKeys.forEach(key => {
            if (vector2Fields.has(key)) {
                // For Vector2 fields, add separate columns for x, y, and magnitude
                dataHeaders.push(`data_${key}_x`);
                dataHeaders.push(`data_${key}_y`);
                dataHeaders.push(`data_${key}_magnitude`); // Add magnitude for easier analysis
            } else {
                // For standard fields (including the wrapped primitives under 'value')
                dataHeaders.push(`data_${key}`);
            }
        });
        const headers = baseHeaders.concat(dataHeaders);
        // --- End Header Creation ---


        // --- Clear Existing Data and Set Headers ---
        // Clear only the active data sheet
        dataSheet.clearContents(); // Use clearContents instead of clear to keep formatting
        dataSheet.appendRow(headers);
        // Deletion of chart sheets is now handled solely in createChartsForMetrics
        // --- End Clear Data ---


        // --- Row Creation ---
        let rows = [];
        metricEntries.forEach(entry => {
            const row = [
                entry.gameVersion,
                Utilities.formatDate(entry.timestamp, SpreadsheetApp.getActiveSpreadsheet().getSpreadsheetTimeZone(), "MM-dd-yyyy HH:mm:ss"), // Use sheet timezone
                entry.deviceId,
                entry.sessionId,
                entry.metricId,
                JSON.stringify(entry.rawData) // Store the processed JSON
            ];

            // Add columns for each top-level key
            topLevelKeys.forEach(key => {
                const value = entry.rawData.hasOwnProperty(key) ? entry.rawData[key] : undefined;

                if (vector2Fields.has(key)) {
                    // Handle Vector2 fields
                    if (typeof value === 'object' && value !== null && 'x' in value && 'y' in value) {
                        const x = Number(value.x);
                        const y = Number(value.y);
                        const magnitude = Math.sqrt(x * x + y * y);
                        row.push(x);
                        row.push(y);
                        row.push(magnitude);
                    } else {
                        // Key is vector type, but data is missing/malformed for this entry
                        row.push("", "", "");
                    }
                } else {
                    // Handle standard fields
                    if (value !== undefined) {
                        // If value is an object (but not identified as Vector2), stringify it; otherwise, use as is
                        row.push(typeof value === 'object' ? JSON.stringify(value) : value);
                    } else {
                        // Key exists overall, but not in this specific entry's rawData
                        row.push("");
                    }
                }
            });
            rows.push(row);
        });
        // --- End Row Creation ---

        // Sort rows based on the original timestamp Date objects
        // Create an array of objects to sort, keeping row data and timestamp together
        const sortableRows = rows.map((row, index) => ({
            rowData: row,
            timestamp: metricEntries[index].timestamp
        }));

        sortableRows.sort((a, b) => a.timestamp - b.timestamp);

        // Extract the sorted row data
        const sortedRows = sortableRows.map(item => item.rowData);


        // Append all rows to the sheet
        if (sortedRows.length > 0) {
            // Ensure all rows have the same number of columns as the headers
            const numColumns = headers.length;
            const preparedRows = sortedRows.map(row => {
                const fullRow = row.slice(0, numColumns); // Take only needed columns
                while (fullRow.length < numColumns) {
                    fullRow.push(""); // Pad with empty strings if needed
                }
                return fullRow;
            });
            dataSheet.getRange(2, 1, preparedRows.length, numColumns).setValues(preparedRows);
            Logger.log(`Successfully wrote ${preparedRows.length} rows to sheet.`);
        } else {
            Logger.log(`No rows to write to the sheet.`);
        }

        // Auto-resize columns for better readability
        if (headers.length > 0) {
            dataSheet.autoResizeColumns(1, headers.length);
        }

        Logger.log(`Imported ${sortedRows.length} metric records with ${topLevelKeys.length} data fields from game version ${gameVersion}.`);

        // Create charts for each unique metric ID
        createChartsForMetrics(spreadsheet, dataSheet, Array.from(uniqueMetricIds), topLevelKeys, vector2FieldsArray);

    } catch (error) {
        ui.alert('Error', `Failed to import data from game version ${gameVersion}: ${error.toString()}\n${error.stack}`, ui.ButtonSet.OK);
        Logger.log(`Error: ${error.toString()}\nStack: ${error.stack}`);
    }
}

// Function to create charts for each metric
function createChartsForMetrics(spreadsheet, dataSheet, metricIds, dataKeys, vector2Fields) {
    Logger.log(`Starting chart creation for metrics: ${metricIds.join(', ')}`);

    // --- Delete any existing chart sheets ---
    const sheets = spreadsheet.getSheets();
    let deletedCount = 0;
    for (let i = sheets.length - 1; i >= 0; i--) { // Iterate backwards when deleting
        const sheet = sheets[i];
        const sheetName = sheet.getName();
        // Ensure we don't delete the main data sheet or other unrelated sheets
        if (sheetName.startsWith('Chart_') && sheetName !== dataSheet.getName()) {
            try {
                spreadsheet.deleteSheet(sheet);
                deletedCount++;
            } catch (e) {
                Logger.log(`Warning: Failed to delete existing chart sheet ${sheetName}: ${e.toString()}`);
                // Continue trying to delete others
            }
        }
    }
    if (deletedCount > 0) {
        Logger.log(`Deleted ${deletedCount} existing chart sheets.`);
        SpreadsheetApp.flush(); // IMPORTANT: Apply deletions immediately
        Utilities.sleep(1000); // Add a small pause just in case flush needs time
    }
    // --- End Deletion ---

    // Get data once
    const dataRange = dataSheet.getDataRange();
    const allValues = dataRange.getValues(); // Get all data including headers
    if (allValues.length < 2) {
        Logger.log("No data rows found in the data sheet for charting.");
        return; // No data to chart
    }
    const headers = allValues[0];
    const metricIdColIndex = headers.indexOf("Metric ID");
    const timestampColIndex = headers.indexOf("Timestamp"); // Still useful for filtering rows if needed, but not for histogram axis

    if (metricIdColIndex === -1) {
        Logger.log("Required column ('Metric ID') not found in data sheet headers.");
        return;
    }

    // Create charts for each metric
    metricIds.forEach(metricId => {
        // Skip if metricId is empty or null
        if (!metricId) {
            Logger.log("Skipping chart creation for empty metric ID.");
            return;
        }
        Logger.log(`Processing charts for Metric ID: ${metricId}`);

        // Create a new sheet for this metric's charts
        const safeMetricId = metricId.toString().replace(/[^\w\s-]/gi, '_'); // Allow hyphens, replace other non-word chars
        const chartSheetName = `Chart_${safeMetricId}`.substring(0, 100); // Max sheet name length is 100
        let chartSheet;
        try {
            chartSheet = spreadsheet.insertSheet(chartSheetName);
            Logger.log(`Created new chart sheet: ${chartSheetName}`);
        } catch (e) {
            // If sheet name already exists (e.g., due to length truncation collision), append a number/UUID
            try {
                const uniqueSuffix = Utilities.getUuid().substring(0, 4);
                const fallbackName = `Chart_${safeMetricId.substring(0, 95)}_${uniqueSuffix}`;
                chartSheet = spreadsheet.insertSheet(fallbackName);
                Logger.log(`Created new chart sheet with fallback name: ${chartSheet.getName()}`);
            } catch (e2) {
                Logger.log(`Error creating chart sheet for ${metricId}: ${e.toString()} and fallback failed: ${e2.toString()}. Skipping charts for this metric.`);
                return; // Cannot create sheet, skip this metric
            }
        }

        // Set up chart sheet header
        chartSheet.getRange(1, 1).setValue(`Charts for Metric: ${metricId}`).setFontWeight('bold').setFontSize(14);

        // Find row indices in the data sheet that match this metric ID
        const metricRowIndices = []; // Store 0-based indices for 'allValues' array
        for (let i = 1; i < allValues.length; i++) { // Start from 1 to skip header row
            if (allValues[i][metricIdColIndex] === metricId) {
                metricRowIndices.push(i);
            }
        }

        if (metricRowIndices.length === 0) {
            chartSheet.getRange(2, 1).setValue(`No data found for metric "${metricId}" in the data sheet.`);
            Logger.log(`No data rows found for metric ${metricId}.`);
            return;
        }
        Logger.log(`Found ${metricRowIndices.length} data rows for metric ${metricId}.`);

        // Chart positioning variables
        let chartAnchorRow = 3; // Starting row for charts/temp data
        const chartInsertCol = 5; // Column where charts will be inserted
        const tempCol = 1;       // Column for temporary data

        // --- Process standard fields (potential histograms) ---
        dataKeys.forEach(key => {
            // Skip keys that were identified as Vector2 fields
            if (vector2Fields.includes(key)) return;

            const dataKeyColName = `data_${key}`;
            const dataKeyColIndex = headers.indexOf(dataKeyColName);

            if (dataKeyColIndex === -1) {
                Logger.log(`Data column '${dataKeyColName}' not found for metric '${metricId}'. Skipping chart for this key.`);
                return; // Column for this key doesn't exist
            }
            Logger.log(`Processing key '${key}' (column ${dataKeyColIndex}) for potential histogram/chart for metric ${metricId}`);

            // Extract *only the values* for this metric and key
            // We need an array of arrays for setValues: [[val1], [val2], ...]
            let rawValues = [];
            metricRowIndices.forEach(rowIndex => {
                const val = allValues[rowIndex][dataKeyColIndex];
                if (val !== "" && val !== null && val !== undefined) {
                    rawValues.push(val);
                }
            });

            // If no valid data, skip
            if (rawValues.length === 0) {
                Logger.log(`No valid data found for key '${key}' for metric '${metricId}'. Skipping chart.`);
                return;
            }

            // Check whether data is numeric. If all values can be converted to numbers then treat as numeric.
            let isNumeric = rawValues.every(val => !isNaN(Number(val)));

            if (isNumeric) {
                // For numeric data, continue to build numeric histogram
                let histogramDataValues = [];
                rawValues.forEach(val => {
                    histogramDataValues.push([Number(val)]);
                });
                Logger.log(`Found ${histogramDataValues.length} numeric data points for key '${key}'. Creating Histogram.`);

                // Create temporary range in the chart sheet for numeric histogram (single column)
                const tempDataRows = histogramDataValues.length + 1; // +1 for header
                const tempDataRange = chartSheet.getRange(chartAnchorRow, tempCol, tempDataRows, 1);
                tempDataRange.setValues([[key]].concat(histogramDataValues));

                // Build histogram chart for numeric values
                const chartBuilder = chartSheet.newChart()
                    .setChartType(Charts.ChartType.HISTOGRAM)
                    .addRange(tempDataRange)
                    .setOption('title', `Distribution of ${formatKey(key)}`)
                    .setOption('legend', { position: 'none' })
                    .setOption('hAxis', { title: key })
                    .setOption('vAxis', { title: 'Frequency' })
                    .setOption('width', 600)
                    .setOption('height', 400)
                    .setOption('aggregate', true)
                    .setPosition(chartAnchorRow, chartInsertCol, 0, 0);

                chartSheet.insertChart(chartBuilder.build());
                Logger.log(`Inserted Histogram chart for key '${key}'.`);
                chartAnchorRow += tempDataRows + 2; // Advance for next chart

            } else {
                // For non-numeric data (words): build a frequency table.
                let freqMap = {};
                rawValues.forEach(val => {
                    // Convert value to string explicitly (in case it's stored otherwise)
                    const strVal = String(val);
                    freqMap[strVal] = (freqMap[strVal] || 0) + 1;
                });
                Logger.log(`Found frequency data for key '${key}': ${JSON.stringify(freqMap)}`);

                // Prepare frequency data array for charting
                // Header row: [key, 'Count']
                let frequencyData = [];
                for (let word in freqMap) {
                    frequencyData.push([word, freqMap[word]]);
                }
                const tempDataRows = frequencyData.length;
                const tempDataCols = 2; // Two columns: word and count

                // Create a temporary range for frequency data.
                const tempDataRange = chartSheet.getRange(chartAnchorRow, tempCol, tempDataRows, tempDataCols);
                tempDataRange.setValues(frequencyData);

                // Create a column chart (bar chart) with the string frequency data.
                // The first column will be used for the domain (words)
                const chartBuilder = chartSheet.newChart()
                    .setChartType(Charts.ChartType.COLUMN)
                    .addRange(tempDataRange)
                    .setOption('title', `Frequency of ${formatKey(key)}`)
                    .setOption('hAxis', { title: key })
                    .setOption('vAxis', { title: 'Frequency' })
                    .setOption('legend', { position: 'none' })
                    .setOption('width', 600)
                    .setOption('height', 400)
                    .setPosition(chartAnchorRow, chartInsertCol, 0, 0);

                chartSheet.insertChart(chartBuilder.build());
                Logger.log(`Inserted Column chart for string data key '${key}'.`);
                chartAnchorRow += tempDataRows + 2; // Advance anchor row for next chart
            }
        }); // --- End standard key loop ---

        // --- Process Vector2 fields (Scatter Plots - unchanged) ---
        vector2Fields.forEach(key => {
            const dataKeyXColName = `data_${key}_x`;
            const dataKeyYColName = `data_${key}_y`;
            const dataKeyXColIndex = headers.indexOf(dataKeyXColName);
            const dataKeyYColIndex = headers.indexOf(dataKeyYColName);

            if (dataKeyXColIndex === -1 || dataKeyYColIndex === -1) {
                Logger.log(`Vector columns ('${dataKeyXColName}', '${dataKeyYColName}') not found for key '${key}', metric '${metricId}'. Skipping scatter chart.`);
                return; // Columns for this vector key don't exist
            }
            Logger.log(`Charting vector key '${key}' (columns ${dataKeyXColIndex}, ${dataKeyYColIndex}) for metric ${metricId}`);


            // Collect valid X, Y pairs for this metric
            let xyPairs = [];
            metricRowIndices.forEach(rowIndex => {
                const xVal = allValues[rowIndex][dataKeyXColIndex];
                const yVal = allValues[rowIndex][dataKeyYColIndex];
                // Ensure both x and y are numeric
                if (xVal !== "" && xVal !== null && !isNaN(Number(xVal)) &&
                    yVal !== "" && yVal !== null && !isNaN(Number(yVal))) {
                    xyPairs.push([Number(xVal), Number(yVal)]);
                }
            });

            if (xyPairs.length > 0) {
                Logger.log(`Found ${xyPairs.length} valid X,Y pairs for vector key '${key}'. Creating Scatter chart.`);
                // Create range for scatter data
                const scatterDataRows = xyPairs.length + 1; // +1 for header
                const scatterDataRange = chartSheet.getRange(chartAnchorRow, tempCol, scatterDataRows, 2);

                // Populate the range with header and data
                scatterDataRange.setValues([[`${key}_X`, `${key}_Y`]].concat(xyPairs));

                // Create scatter chart
                const scatterChartBuilder = chartSheet.newChart()
                    .setChartType(Charts.ChartType.SCATTER)
                    .addRange(scatterDataRange)
                    .setOption('useFirstColumnAsDomain', true) // X column is the domain
                    .setOption('title', `${formatKey(key)} (X-Y Plot)`)
                    .setOption('legend', { position: 'none' })
                    .setOption('hAxis', { title: `${key} X` })
                    .setOption('vAxis', { title: `${key} Y` })
                    .setOption('pointSize', 5)
                    .setOption('width', 600)
                    .setOption('height', 600)
                    .setPosition(chartAnchorRow, chartInsertCol, 0, 0); // Anchor near data

                chartSheet.insertChart(scatterChartBuilder.build());
                Logger.log(`Inserted Scatter chart for key '${key}'.`);

                // Move anchor row for the next chart/data block
                chartAnchorRow += scatterDataRows + 2; // Add space below the temp data
            } else {
                Logger.log(`No valid X,Y pairs found for vector key '${key}' for metric '${metricId}'. Skipping Scatter chart.`);
            }
        }); // --- End Vector2 key loop ---

        // Hide the temporary data columns if desired (optional)
        // chartSheet.hideColumns(tempCol, 2); // Hides columns A and B (or just A now for histogram)

        // Auto-resize columns in the chart sheet where charts are placed
        if (chartInsertCol > 0) {
            chartSheet.autoResizeColumns(chartInsertCol, 5); // Resize a few columns around the chart insertion point
        }
        Logger.log(`Finished chart processing for Metric ID: ${metricId}`);

    }); // --- End metricId loop ---
    Logger.log("Finished creating all charts.");
}

// Add a menu item to run the import (Keep this function as is)
function onOpen() {
    SpreadsheetApp.getUi()
        .createMenu('Firebase Metrics') // Renamed menu for clarity
        .addItem('Import & Chart Data', 'importFirebaseData')
        .addToUi();
}

// Make sure the importFirebaseData function is also present in your script.gs file.
// (The original importFirebaseData function you provided is correct and doesn't need changes for this request)