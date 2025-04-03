// This is a Google Apps Script that imports data from a Firebase Realtime Database into a Google Sheet.
// The script prompts the user to enter a game version number, fetches the data from Firebase, and populates the sheet with the data.
// The script also creates charts for each metric automatically.

function importFirebaseData() {
    const ui = SpreadsheetApp.getUi();
    const spreadsheet = SpreadsheetApp.getActiveSpreadsheet();
    const sheet = spreadsheet.getActiveSheet();

    const projectId = "team-second-chance";
    const apiKey = "AIzaSyAoXTy6p6rtSSHtduwgQ86zpAdlCNtq08w";

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
        const response = UrlFetchApp.fetch(url);
        const data = JSON.parse(response.getContentText());

        if (!data || Object.keys(data).length === 0) {
            ui.alert('No Data', `No data found for game version ${gameVersion}.`, ui.ButtonSet.OK);
            return;
        }

        const metricEntries = [];
        const allTopLevelKeys = new Set();
        const uniqueMetricIds = new Set();

        for (const deviceId in data) {
            // Iterate through session IDs for this device
            for (const sessionId in data[deviceId]) {
                // Iterate through metric IDs for this session
                for (const metricId in data[deviceId][sessionId]) {
                    uniqueMetricIds.add(metricId); // Add to our set of unique metrics

                    // Iterate through entries for this metric
                    for (const entryId in data[deviceId][sessionId][metricId]) {
                        const entry = data[deviceId][sessionId][metricId][entryId];
                        const timestamp = entry.timestamp;

                        // Store the raw metric data object and discover all keys
                        let metricData = {};
                        if (typeof entry.data === 'object' && entry.data !== null) {
                            metricData = entry.data;

                            // Collect all top-level keys
                            Object.keys(metricData).forEach(key => allTopLevelKeys.add(key));
                        }

                        metricEntries.push({
                            gameVersion: gameVersion,
                            timestamp: new Date(timestamp), // Keep as Date object for sorting
                            deviceId: deviceId,
                            sessionId: sessionId,
                            metricId: metricId,
                            rawData: entry.data
                        });
                    }
                }
            }
        }

        // Convert the set of keys to an array and sort alphabetically
        const topLevelKeys = Array.from(allTopLevelKeys).sort();

        // Create headers - base columns plus data_{key} for each top level key
        const headers = ["Game Version", "Timestamp", "Device ID", "Session ID", "Metric ID", "Raw Data"];
        topLevelKeys.forEach(key => {
            headers.push(`data_${key}`);
        });

        // Clear existing data and set headers
        sheet.clear();
        sheet.appendRow(headers);

        // Create rows with extracted data
        const rows = [];

        metricEntries.forEach(entry => {
            const row = [
                entry.gameVersion,
                Utilities.formatDate(entry.timestamp, "GMT", "MM-dd-yyyy HH:mm:ss"),
                entry.deviceId,
                entry.sessionId,
                entry.metricId,
                JSON.stringify(entry.rawData)
            ];

            // Add columns for each top-level key
            topLevelKeys.forEach(key => {
                if (typeof entry.rawData === 'object' && entry.rawData !== null && key in entry.rawData) {
                    const value = entry.rawData[key];
                    // If value is an object, stringify it; otherwise, use as is
                    row.push(typeof value === 'object' ? JSON.stringify(value) : value);
                } else {
                    row.push(""); // Empty if key doesn't exist in this entry
                }
            });

            rows.push(row);
        });

        // Sort by timestamp (oldest first)
        rows.sort((a, b) => {
            return metricEntries[rows.indexOf(a)].timestamp - metricEntries[rows.indexOf(b)].timestamp;
        });

        // Append all rows to the sheet
        if (rows.length > 0) {
            sheet.getRange(2, 1, rows.length, rows[0].length).setValues(rows);
        }

        // Auto-resize columns for better readability
        sheet.autoResizeColumns(1, headers.length);

        Logger.log(`Imported ${rows.length} metric records with ${topLevelKeys.length} data fields from game version ${gameVersion}.`);

        // Create charts for each unique metric ID
        createChartsForMetrics(spreadsheet, sheet, Array.from(uniqueMetricIds), topLevelKeys);

    } catch (error) {
        ui.alert('Error', `Failed to import data from game version ${gameVersion}: ${error.toString()}`, ui.ButtonSet.OK);
        Logger.log(`Error: ${error.toString()}`);
    }
}

// Function to create charts for each metric
function createChartsForMetrics(spreadsheet, dataSheet, metricIds, dataKeys) {
    // Delete any existing chart sheets
    const sheets = spreadsheet.getSheets();
    for (let i = 0; i < sheets.length; i++) {
        const sheet = sheets[i];
        if (sheet.getName().startsWith('Chart_')) {
            spreadsheet.deleteSheet(sheet);
        }
    }

    // Create charts for each metric
    metricIds.forEach(metricId => {
        // Skip if metricId is empty
        if (!metricId) return;

        // Create a new sheet for this metric's charts
        const chartSheetName = `Chart_${metricId}`.replace(/[^\w\s]/gi, '_').substring(0, 30); // Safe sheet name
        let chartSheet;
        try {
            chartSheet = spreadsheet.insertSheet(chartSheetName);
        } catch (e) {
            // If sheet already exists (unlikely after our cleanup), get it
            chartSheet = spreadsheet.getSheetByName(chartSheetName);
            chartSheet.clear();
        }

        // Set up chart sheet header
        chartSheet.getRange(1, 1).setValue(`Charts for Metric: ${metricId}`);
        chartSheet.getRange(1, 1).setFontWeight('bold').setFontSize(14);

        // Find rows in the data sheet that match this metric ID
        const dataRange = dataSheet.getDataRange();
        const values = dataRange.getValues();
        const metricIdColIndex = values[0].indexOf("Metric ID");
        const timestampColIndex = values[0].indexOf("Timestamp");

        if (metricIdColIndex === -1 || timestampColIndex === -1) {
            Logger.log("Required columns not found in data sheet");
            return;
        }

        // Determine which rows contain data for this metric
        const metricRows = [];
        for (let i = 1; i < values.length; i++) {
            if (values[i][metricIdColIndex] === metricId) {
                metricRows.push(i + 1); // +1 because array is 0-indexed but sheets are 1-indexed
            }
        }

        if (metricRows.length === 0) {
            chartSheet.getRange(2, 1).setValue(`No data found for this metric.`);
            return;
        }

        // For each data key, create a chart if the data is numeric
        let chartRow = 3; // Starting row for charts
        let chartCol = 1; // Starting column for charts

        dataKeys.forEach(key => {
            const dataKeyColIndex = values[0].indexOf(`data_${key}`);
            if (dataKeyColIndex === -1) return;

            // Check if this data key has numeric values for this metric
            let hasNumericValues = false;
            let allValues = [];

            for (let i = 0; i < metricRows.length; i++) {
                const rowIdx = metricRows[i] - 1; // Convert back to 0-indexed for values array
                const val = values[rowIdx][dataKeyColIndex];

                // Try to parse as number
                if (val !== "" && val !== null && !isNaN(Number(val))) {
                    hasNumericValues = true;
                    allValues.push({
                        timestamp: values[rowIdx][timestampColIndex],
                        value: Number(val)
                    });
                }
            }

            if (hasNumericValues && allValues.length > 0) {
                // Create a temporary range in the chart sheet for this data series
                const tempDataRows = allValues.length + 1; // +1 for header
                const tempDataRange = chartSheet.getRange(chartRow, chartCol, tempDataRows, 2);

                // Populate the range with timestamp and value data
                const tempData = [["Timestamp", key]];
                allValues.forEach(item => {
                    tempData.push([item.timestamp, item.value]);
                });
                tempDataRange.setValues(tempData);

                // Create a chart for this key
                const chart = chartSheet.newChart()
                    .setChartType(Charts.ChartType.LINE)
                    .addRange(tempDataRange)
                    .setPosition(chartRow, chartCol + 3, 0, 0)
                    .setOption('title', `${metricId}: ${key} over Time`)
                    .setOption('legend', { position: 'none' })
                    .setOption('width', 600)
                    .setOption('height', 400)
                    .build();

                chartSheet.insertChart(chart);

                // Move to next position for charts (creating a grid layout)
                chartRow += tempDataRows + 2; // Add space between data areas

                // If we're getting too far down, reset row and move to next column
                if (chartRow > 50) {
                    chartRow = 3;
                    chartCol += 10; // Move over several columns
                }
            }
        });

        // Auto-resize columns in the chart sheet
        chartSheet.autoResizeColumns(1, 5);
    });
}

// Add a menu item to run the import
function onOpen() {
    SpreadsheetApp.getUi()
        .createMenu('Firebase')
        .addItem('Import Metrics Data', 'importFirebaseData')
        .addToUi();
}