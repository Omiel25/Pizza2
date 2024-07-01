let rows = document.querySelectorAll(".row");
let rowsTopWidth = 0;

console.log("event firing");
rows.forEach(row => {
    let inputLabels = row.querySelectorAll(".input-group span");
    inputLabels.forEach(label => {
        let labelWidth = label.getBoundingClientRect().width
        if (rowsTopWidth < labelWidth) {
            rowsTopWidth = labelWidth;
        }
    });
});

rows.forEach(row => {
    let inputLabels = row.querySelectorAll(".input-group span");
    inputLabels.forEach(label => {
        label.style.width = rowsTopWidth + "px";
    });
});