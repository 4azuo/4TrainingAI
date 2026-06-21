/* Báo cáo Test — render từ report-data.json (sinh bởi scripts/13-generate-report.ps1).
   Mở report qua report-server.py (http://127.0.0.1:8770/new/<ts>/report.html) để tải JSON,
   hiển thị ảnh evidence và dùng nút Done. Tính năng: tab UT/IT/Tổng hợp · search highlight +
   next/prev (Enter/Shift+Enter) · Done (POST /api/done). */
(function () {
  "use strict";

  // Dữ liệu nhúng sẵn (report-data.js) -> report mở trực tiếp bằng file:// vẫn chạy.
  var D = window.REPORT_DATA || { meta: {}, ut: { files: [], uncovered: [] }, it: { scenarios: [], functions: [] } };
  main(D);

  function main(D) {

    function esc(s) {
      return String(s == null ? "" : s)
        .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;");
    }
    function covClass(p) { return p == null ? "" : (p >= 80 ? "hi" : (p >= 50 ? "mid" : "lo")); }
    function covText(p) { return p == null ? "—" : (p + "%"); }
    function $(id) { return document.getElementById(id); }

    /* ---------------- Header ---------------- */
    $("reportTitle").textContent = D.meta.title || "Báo cáo Test";
    $("reportDate").textContent = "📅 " + (D.meta.date || "") + " · RestaurantApp" +
      (D.meta.runId ? " · " + D.meta.runId : "");
    document.title = (D.meta.title || "Báo cáo Test") + " — RestaurantApp";

    /* ---------------- UT tab ---------------- */
    function renderUT() {
      var ut = D.ut || {}, files = ut.files || [];
      var html = "";
      html += '<h2>Unit Test theo file nguồn (' + (ut.total || 0) + ' test · coverage tổng ' +
        covText(ut.totalCoverage) + ')</h2>';
      html += '<table><thead><tr><th>File nguồn</th><th>Test class</th>' +
        '<th>Số test</th><th>Pass/Fail</th><th>Coverage</th></tr></thead><tbody>';
      if (!files.length) html += '<tr><td colspan="5" class="muted">Không có dữ liệu UT.</td></tr>';
      files.forEach(function (f, i) {
        var p = covClass(f.coverage);
        html += '<tr class="file-row" data-detail="ut-' + i + '">' +
          '<td><span class="caret">▶</span> ' + esc(f.srcFile) + '</td>' +
          '<td class="muted">' + esc(f.testClass) + '</td>' +
          '<td>' + f.count + '</td>' +
          '<td><span class="badge pass">' + f.passed + '</span> ' +
          (f.failed ? '<span class="badge fail">' + f.failed + '</span>' : '') + '</td>' +
          '<td><span class="cov ' + p + '">' + covText(f.coverage) + '</span>' +
          (f.coverage != null ? '<div class="cov-bar"><i style="width:' + f.coverage + '%"></i></div>' : '') +
          '</td></tr>';
        var inner = '';
        (f.tests || []).forEach(function (t) {
          var b = t.outcome === "Passed" ? "pass" : (t.outcome === "Failed" ? "fail" : "na");
          inner += '<div class="method-line"><span class="method-name">' + esc(t.method) +
            '</span><span><span class="badge ' + b + '">' + esc(t.outcome) + '</span> ' +
            '<span class="dur">' + t.durationMs + ' ms</span></span></div>';
        });
        html += '<tr class="detail-row hidden" id="ut-' + i + '"><td colspan="5">' +
          '<div class="detail-inner">' + inner + '</div></td></tr>';
      });
      html += '</tbody></table>';

      var unc = ut.uncovered || [];
      if (unc.length) {
        html += '<h2>File nguồn khác (có coverage, không có UT trực tiếp)</h2>';
        html += '<table><thead><tr><th>File nguồn</th><th>Coverage</th></tr></thead><tbody>';
        unc.forEach(function (u) {
          html += '<tr><td>' + esc(u.srcFile) + '</td><td><span class="cov ' + covClass(u.coverage) +
            '">' + covText(u.coverage) + '</span></td></tr>';
        });
        html += '</tbody></table>';
      }
      $("tab-ut").innerHTML = html;
    }

    /* ---------------- IT tab ---------------- */
    function renderIT() {
      var it = D.it || {}, scenarios = it.scenarios || [];
      var html = '<h2>Integration Test (' + (it.total || 0) + ' testcase · ' +
        (it.passed || 0) + ' pass / ' + (it.failed || 0) + ' fail)</h2>';
      if (it.functions && it.functions.length) {
        html += '<div class="chips">' + it.functions.map(function (fn) {
          return '<span class="chip">' + esc(fn) + '</span>';
        }).join('') + '</div>';
      }
      html += '<table><thead><tr><th>Scenario</th><th>Testcase</th></tr></thead><tbody>';
      if (!scenarios.length) html += '<tr><td colspan="2" class="muted">Không có dữ liệu IT.</td></tr>';
      var k = 0;
      scenarios.forEach(function (sc) {
        var first = true;
        (sc.cases || []).forEach(function (c) {
          var id = "it-" + (k++);
          var b = c.result === "PASS" ? "pass" : (c.result === "FAIL" ? "fail" : "na");
          html += '<tr class="it-case file-row" data-detail="' + id + '">' +
            '<td>' + (first ? '<span class="muted">' + esc(sc.scenarioFile || "") + '</span>' : '') + '</td>' +
            '<td><span class="caret">▶</span> <b>' + esc(c.id) + '</b> ' + esc(c.title) +
            ' &nbsp;<span class="badge ' + b + '">' + esc(c.result) + '</span>' +
            (c.function ? ' <span class="chip">' + esc(c.function) + '</span>' : '') + '</td></tr>';
          var ev = '<div class="it-evidence" id="' + id + '">';
          if (c.steps && c.steps.length) {
            ev += '<h4>Các bước</h4><ul>' + c.steps.map(function (s) { return '<li>' + esc(s) + '</li>'; }).join('') + '</ul>';
          }
          if (c.expected && c.expected.length) {
            ev += '<h4>Kết quả mong đợi</h4><ul>' + c.expected.map(function (s) { return '<li>' + esc(s) + '</li>'; }).join('') + '</ul>';
          }
          if (c.log) ev += '<h4>output.log</h4><pre>' + esc(c.log) + '</pre>';
          // Evidence ảnh trước → sau; fallback c.screenshot cho report cũ (1 ảnh = "sau").
          var before = c.beforeScreenshot || "";
          var after = c.afterScreenshot || c.screenshot || "";
          if (before || after) {
            ev += '<h4>Ảnh evidence (trước → sau)</h4><div class="shots">';
            if (before) ev += '<figure><figcaption>Trước</figcaption>' +
              '<img loading="lazy" src="' + esc(before) + '" alt="' + esc(c.id) + ' trước"></figure>';
            if (after) ev += '<figure><figcaption>Sau</figcaption>' +
              '<img loading="lazy" src="' + esc(after) + '" alt="' + esc(c.id) + ' sau"></figure>';
            ev += '</div>';
          }
          ev += '</div>';
          html += '<tr class="detail-row hidden" id="row-' + id + '"><td colspan="2">' + ev + '</td></tr>';
          first = false;
        });
      });
      html += '</tbody></table>';
      $("tab-it").innerHTML = html;
    }

    /* ---------------- Summarize tab ---------------- */
    function renderSum() {
      var ut = D.ut || {}, it = D.it || {};
      var html = '<h2>Unit Test</h2><div class="stats">' +
        '<div class="stat"><div class="n">' + (ut.total || 0) + '</div><div class="lbl">Tổng test</div></div>' +
        '<div class="stat pass"><div class="n">' + (ut.passed || 0) + '</div><div class="lbl">Passed</div></div>' +
        '<div class="stat fail"><div class="n">' + (ut.failed || 0) + '</div><div class="lbl">Failed</div></div>' +
        '<div class="stat cov"><div class="n">' + covText(ut.totalCoverage) + '</div><div class="lbl">Coverage tổng</div></div>' +
        '</div>';
      html += '<h2>Integration Test</h2><div class="stats">' +
        '<div class="stat"><div class="n">' + (it.total || 0) + '</div><div class="lbl">Tổng testcase</div></div>' +
        '<div class="stat pass"><div class="n">' + (it.passed || 0) + '</div><div class="lbl">Pass</div></div>' +
        '<div class="stat fail"><div class="n">' + (it.failed || 0) + '</div><div class="lbl">Fail</div></div>' +
        '</div>';
      html += '<h2>Chức năng được phủ (IT)</h2>';
      html += (it.functions && it.functions.length)
        ? '<div class="chips">' + it.functions.map(function (f) { return '<span class="chip">' + esc(f) + '</span>'; }).join('') + '</div>'
        : '<p class="muted">—</p>';
      $("tab-sum").innerHTML = html;
    }

    renderUT(); renderIT(); renderSum();

    /* ---------------- Expand / collapse ---------------- */
    document.addEventListener("click", function (e) {
      var row = e.target.closest(".file-row");
      if (!row || !row.dataset.detail) return;
      var detail = document.getElementById("row-" + row.dataset.detail) || document.getElementById(row.dataset.detail);
      if (!detail) return;
      var open = detail.classList.toggle("hidden") === false;
      row.classList.toggle("open", open);
    });

    /* ---------------- Tabs ---------------- */
    var tabs = document.querySelectorAll(".tab");
    tabs.forEach(function (t) {
      t.addEventListener("click", function () {
        tabs.forEach(function (x) { x.classList.remove("active"); });
        t.classList.add("active");
        document.querySelectorAll(".tabpane").forEach(function (p) { p.classList.remove("active"); });
        var pane = $("tab-" + t.dataset.tab);
        pane.classList.add("active");
        runSearch();   // re-highlight trong tab vừa chuyển
      });
    });

    /* ---------------- Search + highlight + navigate ---------------- */
    var input = $("searchInput"), countEl = $("searchCount");
    var matches = [], current = -1;

    function activePane() { return document.querySelector(".tabpane.active"); }

    function unwrap(root) {
      root.querySelectorAll("mark").forEach(function (m) { m.replaceWith(document.createTextNode(m.textContent)); });
      root.normalize();
    }

    function reveal(node) {
      // mở các vùng .hidden chứa match để có thể cuộn tới
      var el = node.parentElement;
      while (el && el !== document.body) {
        if (el.classList && el.classList.contains("hidden")) {
          el.classList.remove("hidden");
          var rowId = el.id;
          var trigger = document.querySelector('[data-detail="' + rowId + '"]') ||
            document.querySelector('[data-detail="' + (rowId || "").replace(/^row-/, "") + '"]');
          if (trigger) trigger.classList.add("open");
        }
        el = el.parentElement;
      }
    }

    function runSearch() {
      var pane = activePane();
      document.querySelectorAll(".tabpane").forEach(unwrap);
      matches = []; current = -1;
      var q = (input.value || "").trim();
      if (q && pane) {
        var ql = q.toLowerCase();
        var walker = document.createTreeWalker(pane, NodeFilter.SHOW_TEXT, {
          acceptNode: function (n) {
            if (!n.nodeValue || n.nodeValue.toLowerCase().indexOf(ql) === -1) return NodeFilter.FILTER_REJECT;
            return NodeFilter.FILTER_ACCEPT;
          }
        });
        var nodes = [];
        while (walker.nextNode()) nodes.push(walker.currentNode);
        nodes.forEach(function (node) {
          var text = node.nodeValue, low = text.toLowerCase(), idx = 0, last = 0;
          var frag = document.createDocumentFragment();
          while ((idx = low.indexOf(ql, last)) !== -1) {
            if (idx > last) frag.appendChild(document.createTextNode(text.slice(last, idx)));
            var m = document.createElement("mark");
            m.textContent = text.slice(idx, idx + q.length);
            frag.appendChild(m); matches.push(m); last = idx + q.length;
          }
          if (last < text.length) frag.appendChild(document.createTextNode(text.slice(last)));
          node.parentNode.replaceChild(frag, node);
        });
      }
      if (matches.length) { current = 0; activate(0); }
      updateCount();
    }

    function updateCount() {
      countEl.textContent = (matches.length ? (current + 1) : 0) + "/" + matches.length;
    }

    function activate(i) {
      matches.forEach(function (m) { m.classList.remove("active"); });
      if (i < 0 || i >= matches.length) return;
      var m = matches[i];
      m.classList.add("active");
      reveal(m);
      m.scrollIntoView({ block: "center", behavior: "smooth" });
      updateCount();
    }

    function go(delta) {
      if (!matches.length) return;
      current = (current + delta + matches.length) % matches.length;
      activate(current);
    }

    var debounce;
    input.addEventListener("input", function () {
      clearTimeout(debounce); debounce = setTimeout(runSearch, 120);
    });
    input.addEventListener("keydown", function (e) {
      if (e.key === "Enter") { e.preventDefault(); go(e.shiftKey ? -1 : 1); }
    });
    $("nextBtn").addEventListener("click", function () { go(1); });
    $("prevBtn").addEventListener("click", function () { go(-1); });

    /* ---------------- Done (lưu trữ) ---------------- */
    var modal = $("doneModal");
    function showModal(title, msg, cmd) {
      $("doneModalTitle").textContent = title;
      $("doneModalMsg").textContent = msg;
      var pre = $("doneModalCmd"), copy = $("doneModalCopy");
      if (cmd) { pre.textContent = cmd; pre.classList.remove("hidden"); copy.classList.remove("hidden"); }
      else { pre.classList.add("hidden"); copy.classList.add("hidden"); }
      modal.classList.remove("hidden");
    }
    // $("doneModalClose").addEventListener("click", function () { modal.classList.add("hidden"); });
    $("doneModalCopy").addEventListener("click", function () {
      var t = $("doneModalCmd").textContent;
      if (navigator.clipboard) navigator.clipboard.writeText(t);
    });

    $("doneBtn").addEventListener("click", function () {
      var runId = (D.meta && D.meta.runId) || "";
      // Gọi server nền (report-server.py) bằng URL TUYỆT ĐỐI + truyền runId (yyyyMMdd-HHmmss)
      // để server biết folder nào và move reports/new/<runId> -> reports/done/<runId>.
      // URL tuyệt đối + CORS nên chạy cả khi report mở bằng file://.
      fetch("http://127.0.0.1:8770/api/done", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ runId: runId })
      }).then(function (res) {
        if (!res.ok) throw new Error("http " + res.status);
        return res.json();
      }).then(function (j) {
        $("doneBtn").disabled = true;
        showModal("Đã lưu trữ ✓", "Report đã được chuyển sang: " + (j.dest || "reports/done/…") +
          ". Bạn có thể đóng cửa sổ này.", null);
      }).catch(function () {
        showModal("Không thể tự lưu trữ",
          "Không gọi được report-server.py (http://127.0.0.1:8770). Hãy chắc server đang chạy " +
          "(tự bật khi mở VSCode) rồi bấm Done lại, hoặc khởi động thủ công:",
          "python scripts/report-server.py");
      });
    });
  } // main
})();
