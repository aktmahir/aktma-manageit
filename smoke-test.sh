#!/usr/bin/env bash
set -euo pipefail

BASE_URL="${1:-http://localhost:8081}"

printf '\n[1/3] Checking app health...\n'
curl -fsS "$BASE_URL/health" | tee /tmp/calendarapp-health.json

echo
printf '\n[2/3] Checking login page and admin login...\n'
curl -fsSL -c /tmp/calendarapp.cookies "$BASE_URL/Account/Login" > /tmp/calendarapp-login-page.html
grep -q "Email" /tmp/calendarapp-login-page.html
token=$(grep -o 'name="__RequestVerificationToken" type="hidden" value="[^"]*"' /tmp/calendarapp-login-page.html | sed 's/.*value="//;s/"$//')
curl -fsSL -c /tmp/calendarapp.cookies -b /tmp/calendarapp.cookies -X POST "$BASE_URL/Account/Login" \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  --data-urlencode "__RequestVerificationToken=$token" \
  --data-urlencode 'Email=admin@example.com' \
  --data-urlencode 'Password=AdminPass123!' \
  -D /tmp/calendarapp-login.headers > /tmp/calendarapp-login.html

grep -q '302 Found' /tmp/calendarapp-login.headers
printf '[OK] Admin login succeeded.\n'

echo
printf '\n[3/3] Checking authenticated home page...\n'
curl -fsSL -b /tmp/calendarapp.cookies "$BASE_URL/" | grep -q 'Project kickoff'
printf '[OK] Authenticated homepage renders seeded demo content.\n\n'
printf 'Local demo is ready.\n'
