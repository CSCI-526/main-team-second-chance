// Google Apps Script: Import & chart Firebase RTDB metrics
function formatKey(k) {
    return k.split("_").map(w => w[0].toUpperCase() + w.slice(1)).join(" ");
}

function importFirebaseData() {
    const ui = SpreadsheetApp.getUi(), sh = SpreadsheetApp.getActiveSheet();
    const ver = ui.prompt('Select Game Version', 'Enter version:', ui.ButtonSet.OK_CANCEL);
    if (ver.getSelectedButton() != ui.Button.OK) return;

    const v = ver.getResponseText().trim();
    if (!v || isNaN(v)) return ui.alert('Error', 'Invalid version', ui.ButtonSet.OK);

    const { projectId, apiKey } = { projectId: 'team-second-chance', apiKey: 'AIzaSyAoXTy6p6rtSSHtduwgQ86zpAdlCNtq08w' };
    const url = `https://${projectId}-default-rtdb.firebaseio.com/${v}.json?auth=${apiKey}`;
    const resp = UrlFetchApp.fetch(url, { muteHttpExceptions: true }), code = resp.getResponseCode(), txt = resp.getContentText();
    if (code != 200) return ui.alert('Firebase Error', `Code ${code}: ${txt}`, ui.ButtonSet.OK);

    const data = JSON.parse(txt);
    if (!data || !Object.keys(data).length) return ui.alert('No Data', `No data for version ${v}`, ui.ButtonSet.OK);

    // Flatten into entries
    const entries = [];
    const keys = new Set(), vec = new Set();
    Object.entries(data).forEach(([device, dev]) => dev && Object.entries(dev).forEach(([sess, ses]) => ses && Object.entries(ses).forEach(([mId, mList]) => mList && Object.entries(mList).forEach(([eId, e]) => {
        if (!e || e.timestamp == null) return;

        const raw = typeof e.data == 'object' && e.data ? e.data : { value: e.data };
        Object.entries(raw).forEach(([k, val]) => {
            keys.add(k);
            if (val && val.x != null && val.y != null) vec.add(k);
        });

        entries.push({ v, ts: new Date(e.timestamp), device, sess, mId, raw });
    }))));
    if (!entries.length) return ui.alert('No Metric Entries', `No valid metrics for version ${v}`, ui.ButtonSet.OK);

    // Build headers
    const topKeys = [...keys].sort(), vecKeys = [...vec];
    const headers = ['Game Version', 'Timestamp', 'Device ID', 'Session ID', 'Metric ID', 'Raw JSON',
        ...topKeys.flatMap(k => vec.has(k) ? [`data_${k}_x`, `data_${k}_y`, `data_${k}_magnitude`] : [`data_${k}`])
    ];

    sh.clearContents(); sh.appendRow(headers);

    // Build rows
    const rows = entries.map(e => {
        const base = [e.v, Utilities.formatDate(e.ts, SpreadsheetApp.getActive().getSpreadsheetTimeZone(), 'MM-dd-yyyy HH:mm:ss'), e.device, e.sess, e.mId, JSON.stringify(e.raw)];
        const dataCols = topKeys.flatMap(k => {
            const val = e.raw[k];
            if (vec.has(k)) {
                const x = val?.x || ''; const y = val?.y || '';

                return [x, y, x && y ? Math.hypot(+x, +y) : ''];
            }

            return val == null ? [''] : [(typeof val == 'object' ? JSON.stringify(val) : val)];
        });

        return base.concat(dataCols);
    })
        .sort((a, b) => new Date(a[1]) - new Date(b[1]));

    sh.getRange(2, 1, rows.length, headers.length).setValues(rows);
    sh.autoResizeColumns(1, headers.length);

    createCharts(spreadsheet = SpreadsheetApp.getActive(), sh, [...new Set(entries.map(e => e.mId))], topKeys, vecKeys);
}

function createCharts(spreadsheet, dataSheet, metrics, keys, vecKeys) {
    // Delete old chart sheets
    spreadsheet.getSheets().filter(s => s.getName().startsWith('Chart_') && s != dataSheet)
        .forEach(s => spreadsheet.deleteSheet(s));
    const all = dataSheet.getDataRange().getValues(), hdr = all.shift();
    const midx = hdr.indexOf('Metric ID');

    metrics.forEach(m => {
        if (!m) return;
        const sheet = spreadsheet.insertSheet(`Chart_${m.replace(/[^\w-]/g, '_')}`);
        sheet.getRange(1, 1).setValue(`Charts for Metric: ${m}`).setFontWeight('bold');
        const rows = all.filter(r => r[midx] == m);
        if (!rows.length) { sheet.getRange(2, 1).setValue(`No data for ${m}`); return; }
        let rowPtr = 3;
        const hdrMap = hdr.reduce((o, c, i) => (o[c] = i, o), {});
        // Standard keys
        keys.forEach(k => {
            if (vecKeys.includes(k)) return;
            const colName = `data_${k}`, ci = hdrMap[colName];
            const vals = rows.map(r => r[ci]).filter(v => v != '');
            if (!vals.length) return;

            const numeric = vals.every(v => !isNaN(+v));
            const data = numeric ? [['', ...vals.map(v => +v)]] : vals.map(v => [v]);
            const range = sheet.getRange(rowPtr, 1, data.length, data[0].length);
            range.setValues(data);
            const chart = sheet.newChart()
                .setChartType(numeric ? Charts.ChartType.HISTOGRAM : Charts.ChartType.COLUMN)
                .addRange(range)
                .setOption('title',
                    numeric ? `Distribution of ${formatKey(k)}` : `Frequency of ${formatKey(k)}`)
                .setPosition(rowPtr, 5, 0, 0)
                .build();
            sheet.insertChart(chart);
            rowPtr += data.length + 2;
        });

        // Vector2 keys
        vecKeys.forEach(k => {
            const xi = hdrMap[`data_${k}_x`], yi = hdrMap[`data_${k}_y`];
            const pts = rows.map(r => [+r[xi], +r[yi]]).filter(p => p.every(n => !isNaN(n)));
            if (!pts.length) return;

            const data = [[`${k}_X`, `${k}_Y`], ...pts];
            sheet.getRange(rowPtr, 1, data.length, 2).setValues(data);
            const chart = sheet.newChart()
                .setChartType(Charts.ChartType.SCATTER)
                .addRange(sheet.getRange(rowPtr, 1, data.length, 2))
                .setOption('title', `${formatKey(k)} (X-Y Plot)`)
                .setPosition(rowPtr, 5, 0, 0)
                .build();

            sheet.insertChart(chart);
            rowPtr += data.length + 2;
        });
    });
}

function onOpen() {
    SpreadsheetApp.getUi().createMenu('Firebase Metrics').addItem('Import & Chart Data', 'importFirebaseData').addToUi();
}
