(function ($) {
    var
        lineHeight = 0,
        crrntLine = 0,
        crrntWord = 0,
        crrntLineOffset = 0,
        needSuffix = 0;

    /*
     * Line
     * @param {Object} element
     * @param {Number} lineNumber
     * @param {String} endStr
     */
    $.fn.line = function (lineNumber, endStr) {
        var targets = this;

        targets.each(function (i, e) {
            var element = $(e),
                txt = element.text(),
                editedLines = '';

            // Check for Default Values
            lineNumber = lineNumber ? lineNumber : 2;
            endStr = endStr ? endStr : '...';

            // Check if parent el has 'static' position
            if (element.css('position') === 'static') {
                element.css('position', 'relative');
            }

            // Create storage for initial text
            if (!element.data('initial-content')) {
                element.data('initial-content', txt);
            }

            // Check for initial text length
            if (element.data('initial-content').length >= txt.length) {
                txt = element.data('initial-content');
            }

            element.html(
                wrapWords(txt.split(' ')).join(''));

            editedLines = wrapLines(
                splitToLines(element.find('span'), lineNumber),
                lineNumber);

            element.html(
                addSuffix(editedLines, endStr));

            console.log('Finish!');

            // Refresh
            lineHeight = 0,
                crrntLine = 0,
                crrntWord = 0,
                crrntLineOffset = 0;
        });
    }


    /*
     * Split text content in to lines
     * @param {Array} words
     * @return {Array} arrLines
     */
    function splitToLines(words, lineNumber) {
        var arrLines = [];

        for (
            var i = 0;
            i < words.length;
            i++ , crrntWord++) {
            if (i === 0) {
                arrLines[0] = [];

                if (words.eq(i).position().top !== 0) {
                    crrntLineOffset = words.eq(i).position().top;
                }
            }

            checkOffset = words.eq(i).position().top;
            if (checkOffset > crrntLineOffset) {
                crrntLineOffset = checkOffset;
                crrntLine++;
                crrntWord = 0;
                arrLines[crrntLine] = [];
            }
            arrLines[crrntLine][crrntWord] = words.eq(i);
        }

        // Check if suffix are needed
        if (arrLines[lineNumber]) {
            needSuffix = 1;
        }

        return arrLines;
    }

    /*
     * Wrap Words to spans
     * @private
     * @return {Array} arrWords
     */
    function wrapWords(words) {
        arrWords = [];

        for (var i = words.length; i--;) {
            var crrntWord = words[i].trim();

            arrWords[i] = '<span>' + crrntWord + '</span> ';
        }

        return arrWords;
    }

    /*
     * Wrap Lines to spans
     * @private
     * @param {Array|String} lines
     * @return {String} linesString
     */
    function wrapLines(lines, linesNumber) {
        linesString = '';
        for (
            var i = 0;
            i < linesNumber;
            i++
        ) {
            var line = '';

            // Check if line amount aren't to low
            if (lines[i]) {
                lineLength = lines[i].length;
            } else {
                break;
            }

            for (var j = 0; j < lineLength; j++) {
                line += lines[i][j].text() + ' ';
            }

            linesString += line + '\n';
        }

        return linesString.trim();
    }

    /*
     * Add suffix
     * @private
     * @param {String} str
     * @param {String} suffix
     */
    function addSuffix(str, suffix) {
        if (needSuffix) {
            suffixLen = suffix.length;
            str = str.slice(0, -suffixLen);
            str += suffix;

            // Remove Space before Suffix
            str = str.replace(' ' + suffix, suffix);

            needSuffix = 0;
        }

        return str;
    }
})(jQuery)