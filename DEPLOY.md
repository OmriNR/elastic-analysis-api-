# Deploying to Render

This repo includes `render.yaml`, a [Render Blueprint](https://render.com/docs/blueprint-spec) that
deploys **StoreAPI + frontend + Postgres + MinIO**. Elasticsearch, RabbitMQ, Kibana,
OrdersPublisherAPI, QueueListener, and PublishListener are intentionally left out of this
Blueprint (see "Known limitations" below).

## Deploy steps

1. Push this repo to GitHub/GitLab (Render Blueprints deploy from a connected git repo).
2. In the Render dashboard: **New > Blueprint**, select this repo. Render detects `render.yaml`.
3. Render will prompt for the two secret values that aren't committed to git:
   - `MINIO_ROOT_PASSWORD` (on the `minio` service)
   - `Minio__SecretKey` (on the `storeapi` service)

   **Enter the same password for both** — `Minio__AccessKey`/`MINIO_ROOT_USER` are already
   hardcoded to `storeapi` in `render.yaml`, so this password is the only credential that has
   to match between the two services.
4. Click **Apply** / **Deploy Blueprint**. Render provisions the Postgres database, the MinIO
   private service (with a 10GB disk), then builds and deploys `storeapi` and `frontend`.
5. First boot seeds an admin user (`admin@store.com` / `Admin123!`) and 3 demo products —
   change that password before sharing the URL.

## Known limitations (queues/search skipped by design)

- **No order/product/discount/user "created" events**: `StoreAPI` normally publishes those
  events to RabbitMQ (`RabbitMqProducer`), and no broker is deployed here.
  `RabbitMqProducer` was changed to degrade gracefully — if it can't connect on startup, it logs
  a warning and `PublishMessage` becomes a no-op, instead of throwing. Without this fix,
  `IMessageProducer` being a constructor dependency of nearly every service (Users, Products,
  Orders, Discounts) meant a missing broker broke almost the entire API, not just checkout — this
  was caught by actually running the built image against Postgres+MinIO with no RabbitMQ present.
  Placing an order, creating a product/discount, or registering a user all work; they just don't
  emit an event. Add a RabbitMQ service (e.g. CloudAMQP, or a self-hosted `pserv` like MinIO's)
  and set `Rabbit__Host`/`Rabbit__Port`/`Rabbit__UserName`/`Rabbit__Password` on `storeapi` to
  restore event publishing.
- **No log shipping**: StoreAPI's Serilog sink writes to Elasticsearch at `Elasticsearch__Url`
  (unset here, defaults to `http://localhost:9200`). With nothing listening there, this fails
  silently per-request — it does not affect app functionality, just observability.
- Everything else (auth, products, discounts, users, image upload/serving via MinIO) was verified
  end-to-end against real Postgres + MinIO containers wired the same way `render.yaml` wires them.

## Notes on the Postgres wiring

Render's Postgres exposes host/port/user/password as separate values (its combined
`connectionString` is a `postgres://` URI, which Npgsql doesn't parse), so `storeapi` is wired via
`DB_HOST`/`DB_PORT`/`DB_NAME`/`DB_USER`/`DB_PASSWORD` and assembles the ADO-style connection
string itself (`Program.cs`), defaulting to `SSL Mode=Require`. Override with a `DB_SSL_MODE` env
var if you ever point `storeapi` at a Postgres instance that doesn't support SSL.

## Adjusting cost/scale

Plans in `render.yaml` are minimum-viable defaults: Postgres `free` (time-limited trial — switch
to `basic-256mb`+ for anything long-lived), MinIO `starter` (private services require a paid
plan for disks), frontend `free` (spins down when idle). Edit `render.yaml` or override in the
Render dashboard.
